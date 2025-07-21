using LeaveManagementSystem.Interfaces;
using LeaveManagementSystem.Models;
using LeaveManagementSystem.Models.DTOs;
using LeaveManagementSystem.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace LeaveManagementSystem.Tests.Services
{
    [TestFixture]
    public class LeaveAttachmentServiceTests
    {
        private Mock<IRepository<Guid, LeaveAttachment>> _repositoryMock;
        private Mock<ILeaveRequestService> _leaveRequestServiceMock;
        private Mock<IAuditLogService> _auditLogServiceMock;
        private Mock<ICurrentUserService> _currentUserServiceMock;
        private LeaveAttachmentService _service;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IRepository<Guid, LeaveAttachment>>();
            _leaveRequestServiceMock = new Mock<ILeaveRequestService>();
            _auditLogServiceMock = new Mock<IAuditLogService>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();

            _service = new LeaveAttachmentService(
                _repositoryMock.Object,
                _leaveRequestServiceMock.Object,
                _auditLogServiceMock.Object,
                _currentUserServiceMock.Object);
        }

        [Test]
        public async Task CreateAsync_ValidDto_ReturnsCreatedAttachment()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            var content = "Hello World from a Fake File";
            var fileName = "test.pdf";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;

            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);
            fileMock.Setup(_ => _.CopyToAsync(It.IsAny<Stream>(), default)).Returns((Stream stream, System.Threading.CancellationToken token) =>
            {
                return ms.CopyToAsync(stream);
            });

            var dto = new LeaveAttachmentDto
            {
                LeaveRequestId = Guid.NewGuid(),
                File = fileMock.Object
            };

            var expectedAttachment = new LeaveAttachment
            {
                Id = Guid.NewGuid(),
                LeaveRequestId = dto.LeaveRequestId,
                FileName = fileName,
                FileContent = ms.ToArray(),
                UploadedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _repositoryMock
                .Setup(r => r.Add(It.IsAny<LeaveAttachment>()))
                .ReturnsAsync(expectedAttachment);

            _currentUserServiceMock.Setup(x => x.GetUserId()).Returns(Guid.NewGuid());

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(fileName, result.FileName);
        }

        [Test]
        public async Task GetByIdAsync_ValidId_ReturnsAttachment()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expectedAttachment = new LeaveAttachment
            {
                Id = id,
                FileName = "file.pdf"
            };

            _repositoryMock
                .Setup(r => r.Get(id))
                .ReturnsAsync(expectedAttachment);

            // Act
            var result = await _service.GetByIdAsync(id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("file.pdf", result.FileName);
        }

        [Test]
        public void GetByIdAsync_InvalidId_ThrowsKeyNotFoundException()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repositoryMock.Setup(r => r.Get(id)).ReturnsAsync((LeaveAttachment)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.GetByIdAsync(id));
            Assert.That(ex.Message, Does.Contain("Error occurred while retrieving the leave attachment."));
        }

        [Test]
        public async Task GetByLeaveRequestIdAsync_ValidId_ReturnsAttachments()
        {
            // Arrange
            var leaveRequestId = Guid.NewGuid();

            var leaveRequestResponseDto = new LeaveRequestResponseDto
            {
                Id = leaveRequestId,
                Reason = "Vacation",
                Status = "Approved"
            };

            _leaveRequestServiceMock
                .Setup(s => s.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(leaveRequestResponseDto);

            var attachments = new List<LeaveAttachment>
            {
                new LeaveAttachment
                {
                    Id = Guid.NewGuid(),
                    LeaveRequestId = leaveRequestId,
                    FileName = "file1.pdf",
                    FileContent = new byte[] { 1, 2, 3 },
                    UploadedAt = DateTime.UtcNow,
                    IsDeleted = false
                },
                new LeaveAttachment
                {
                    Id = Guid.NewGuid(),
                    LeaveRequestId = leaveRequestId,
                    FileName = "file2.pdf",
                    FileContent = new byte[] { 4, 5, 6 },
                    UploadedAt = DateTime.UtcNow,
                    IsDeleted = true // Should be skipped
                },
                new LeaveAttachment
                {
                    Id = Guid.NewGuid(),
                    LeaveRequestId = Guid.NewGuid(), // Different request
                    FileName = "file3.pdf",
                    FileContent = new byte[] { 7, 8, 9 },
                    UploadedAt = DateTime.UtcNow,
                    IsDeleted = false
                }
            };

            _repositoryMock
                .Setup(r => r.GetAll())
                .ReturnsAsync(attachments);

            // Act
            var result = await _service.GetByLeaveRequestIdAsync(leaveRequestId);

            // Assert
            var resultList = result.ToList();
            Assert.That(resultList.Count, Is.EqualTo(1));
            Assert.That(resultList[0].FileName, Is.EqualTo("file1.pdf"));
        }

        [Test]
        public async Task DeleteAsync_ValidId_ReturnsTrue()
        {
            // Arrange
            var id = Guid.NewGuid();
            var attachment = new LeaveAttachment
            {
                Id = id
            };

            _repositoryMock
                .Setup(r => r.Delete(id))
                .ReturnsAsync(attachment);

            _currentUserServiceMock.Setup(x => x.GetUserId()).Returns(Guid.NewGuid());

            // Act
            var result = await _service.DeleteAsync(id);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task DeleteAsync_InvalidId_ReturnsFalse()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repositoryMock.Setup(r => r.Delete(id)).ReturnsAsync((LeaveAttachment)null);

            // Act
            var result = await _service.DeleteAsync(id);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
