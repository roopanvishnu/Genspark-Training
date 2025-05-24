-- 7. Create normalized schema for training system

CREATE TABLE Program (
    ProgramID SERIAL PRIMARY KEY,
    ProgramName TEXT NOT NULL
);

CREATE TABLE Course (
    CourseID SERIAL PRIMARY KEY,
    ProgramID INT REFERENCES Program(ProgramID),
    CourseName TEXT NOT NULL
);

CREATE TABLE Trainer (
    TrainerID SERIAL PRIMARY KEY,
    TrainerName TEXT NOT NULL
);

CREATE TABLE Trainee (
    TraineeID SERIAL PRIMARY KEY,
    TraineeName TEXT NOT NULL
);

CREATE TABLE Enrollment (
    EnrollmentID SERIAL PRIMARY KEY,
    TraineeID INT REFERENCES Trainee(TraineeID),
    CourseID INT REFERENCES Course(CourseID),
    EnrollDate DATE DEFAULT CURRENT_DATE
);

CREATE TABLE Certification (
    CertID SERIAL PRIMARY KEY,
    EnrollmentID INT REFERENCES Enrollment(EnrollmentID),
    IssueDate DATE DEFAULT CURRENT_DATE,
    Grade TEXT
);

-- 8. Create function to enroll trainee
CREATE OR REPLACE FUNCTION EnrollTrainee(p_trainee INT, p_course INT)
RETURNS VOID AS $$
BEGIN
    INSERT INTO Enrollment (TraineeID, CourseID)
    VALUES (p_trainee, p_course);
END;
$$ LANGUAGE plpgsql;

-- 9. Create procedure to assign certification
CREATE OR REPLACE PROCEDURE AssignCertification(p_enrollid INT, p_grade TEXT)
LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO Certification (EnrollmentID, Grade)
    VALUES (p_enrollid, p_grade);
END;
$$;
-- 10. Create cursor to list trainees with certifications
CREATE OR REPLACE PROCEDURE ListCertifiedTrainees()
LANGUAGE plpgsql
AS $$
DECLARE
    cert_cursor CURSOR FOR
        SELECT t.TraineeName, c.Grade, cf.IssueDate
        FROM Trainee t
        JOIN Enrollment e ON t.TraineeID = e.TraineeID
        JOIN Certification c ON e.EnrollmentID = c.EnrollmentID
        JOIN Certification cf ON c.CertID = cf.CertID;
    rec RECORD;
BEGIN
    OPEN cert_cursor;
    LOOP
        FETCH cert_cursor INTO rec;
        EXIT WHEN NOT FOUND;
        RAISE NOTICE 'Trainee: %, Grade: %, Date: %', rec.TraineeName, rec.Grade, rec.IssueDate;
    END LOOP;
    CLOSE cert_cursor;
END;
$$;
-- 11. Create roles for Admin, Trainer, and Trainee

CREATE ROLE admin LOGIN PASSWORD 'admin123';
CREATE ROLE trainer LOGIN PASSWORD 'trainer123';
CREATE ROLE trainee LOGIN PASSWORD 'trainee123';

-- 12. Grant role-based permissions
GRANT SELECT, INSERT, UPDATE, DELETE ON Program, Course, Trainer, Trainee, Enrollment, Certification TO admin;
GRANT SELECT, INSERT ON Enrollment TO trainer;
GRANT SELECT ON Program, Course TO trainee;

