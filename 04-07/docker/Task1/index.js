const http = require('http');

const server = http.createServer((req, res) => {
  res.end('Hello there!!');
});

server.listen(3000);