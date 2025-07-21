# first docker file
FROM debian:bullseye

WORKDIR /app

# Install curl and Node.js
RUN apt-get update && \
    apt-get install -y curl && \
    curl -fsSL https://deb.nodesource.com/setup_22.x | bash - && \
    apt-get install -y nodejs

# Copy all files into the image
COPY . .

# Expose the port
EXPOSE 3000

# Start the server
CMD ["node", "server.js"]
