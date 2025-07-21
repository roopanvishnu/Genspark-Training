const express = require('express');
const mongoose = require('mongoose');

const app = express();
const MONGO_URL = 'mongodb://mongo:27017/db';

mongoose.connect(MONGO_URL)
  .then(() => console.log('MongoDB Connected'))
  .catch(err => console.error('MongoDB Connection Failed:', err));

app.get('/', (req, res) => {
  res.send('Hello from Node.js + MongoDB!');
});

app.listen(3000, () => {
  console.log('🚀 Server running on port 3000');
});
