
// import audit from 'express-requests-logger'
//var audit = require('express-requests-logger')

const port = 3000;

const { json } = require('body-parser');
const express = require('express');
const app = express();

const fs = require('node:fs');
const path = require('node:path');

//var dateFormat = require('dateformat');
//import dateFormat, { masks } from "dateformat";

//var bodyParser = require('body-parser')

// parse application/x-www-form-urlencoded
//app.use(bodyParser.urlencoded({ extended: false }))

// parse application/json
//app.use(bodyParser.json())

// Using express.urlencoded middleware
// app.use(express.json()) // for parsing application/json
app.use(express.urlencoded({ extended: true })) // for parsing application/x-www-form-urlencoded

// Body Parser
// https://expressjs.com/en/resources/middleware/body-parser.html

//app.use(audit)

function genDateHourStr()
{
  const now = new Date();
  // The second way: longer but clearer
  const year = now.getFullYear();

  // Warning: from 0 to 11, NOT from 1 to 12
  const month = String(now.getMonth() + 1).padStart(2, '0');

  const day = String(now.getDate()).padStart(2, '0');
  const hour = String(now.getHours()).padStart(2, '0');
  const formattedDateHour = `${year}${month}${day}_${hour}`;
  return formattedDateHour;
}

function createDir(dir){
  // for example: dir = './log'
  if(!fs.existsSync(dir)){
    fs.mkdirSync(dir, { recursive:true } );
  }
}

app.use((req, res, next) => {
/* -------------------------------------------------------------------------
// Para logging
// In this project, we use Middleware function for Logging
// https://expressjs.com/en/guide/writing-middleware.html
// Another option: use the library express-requests-logger
// https://www.npmjs.com/package/express-requests-logger
------------------------------------------------------------------------- */

  try {
    const now = new Date();
    console.log(now);

    //console.log('BEFORE processing request: Do logging!');

    //console.log('now.toISOString():', now.toISOString().substring(0, 13));

    /* should be moved to a utility class with unit test cases */
    /*
    // the first way
    // .replace funcion only replace the first occurance
    var formattedDateHour = String(now.toISOString()).substring(0, 13).replace("-", "").replace("-", "").replace("T", "-");
    */

    /*
    // The second way: longer but clearer
    const year = now.getFullYear();

    // Warning: from 0 to 11, NOT from 1 to 12
    const month = String(now.getMonth() + 1).padStart(2, '0');

    const day = String(now.getDate()).padStart(2, '0');
    const hour = String(now.getHours()).padStart(2, '0');
    const formattedDateHour = `${year}${month}${day}_${hour}`;
    */
    const formattedDateHour = genDateHourStr();

    /*
    // ERROR: does not work
    // Third way: use additional library.
    // https://www.npmjs.com/package/dateformat
    // npm install dateformat
    const formattedDateHour = dateFormat(now, "yyyymmdd-hh");
    */
   
    //console.log('formattedDateHour:', formattedDateHour);

    const logFile = `./log/log_${formattedDateHour}.txt`;

    console.log('BEFORE processing request: log to file', logFile);

    createDir('./log');
    /*
    if(!fs.existsSync('./log')){
      fs.mkdirSync('./log', { recursive:true } );
    }
      */

    // Working with Files 
    // https://nodejs.org/en/learn/manipulating-files/writing-files-with-nodejs

    //console.log(path.dirname(logFile))
    /*
    // to check the absolute path of the log file in server
    var absolutePath = path.resolve(logFile);
    console.log('log File:', absolutePath);
    */

    //fs.writeFileSync('/log.txt', req);
    //fs.writeFileSync('/logSambaBachata.txt', d.toUTCString())

    //fs.writeFileSync(logFile, d.toUTCString());
    //fs.appendFileSync(logFile, '\n' + now.toUTCString());

    fs.appendFileSync(logFile, '\n' + now.toISOString());
    //fs.appendFile(logFile, '\n' + now.toISOString());

    // Get request information
    // https://expressjs.com/en/api.html#req.baseUrl
    // https://expressjs.com/en/api.html#req.originalUrl        
    const protocol = req.protocol;
    //const host = req.hostname; // without port
    const host = req.get('Host') // with port
    const url = req.originalUrl;
    //const port = process.env.PORT || PORT;
    //const fullUrl = `${protocol}://${host}:${port}${url}`

    //const query = req.query; // get URL query parameters

    const fullUrl = `${protocol}://${host}${url}`;

    console.log(fullUrl);

    fs.appendFileSync(logFile, '\n' + fullUrl);
    // fs.appendFile(logFile, '\n' + fullUrl);

    /*
    var body = req.body;

    var theKeys = Object.keys(req.body);
    
    theKeys.forEach(element => {
      console.log(body[element]);      
    });
    */

    /*
    var obj = JSON.parse(result);
var keys = Object.keys(obj);
for (var i = 0; i < keys.length; i++) {
  console.log(obj[keys[i]]);
}
  */

    console.log('Request body is: ', req.body)
    console.log('Each body parameter:')
    for(var key in req.body){
      console.log(key);
    }
    for(var key in req.body){
        var val = req.body[key];
        console.log(key, ":", val);
        //console.log("key:",key);
    }
   
    //var data = req.body;
    //console.log(data);

    const data = JSON.stringify(req.body);
    console.log('JSON data:', data);

    fs.appendFileSync(logFile, '\n' + data);
    // fs.appendFile(logFile, '\n' + data);
  } catch (err) {
    console.error(err);
  }
  
  next();

  try {
    console.log('AFTER processing request: Do nothing now!');
  } catch (err) {
    console.error(err);
  }
});

app.get('/', (req, res) => {

  
  res.send('Hello World!'); // for checking if service is available
})

/*
app.post('/', (req, res)=>{
    res.send('Got a POST request');
})
    */

var curNote = '';

app.post('/note', (req, res) => {
/* -------------------------------------------------------------------------
Read value of the request BODY parameter 'note' and save it to the Current Note variable
Accept empty string input
------------------------------------------------------------------------- */

  //res.send('Got a PUT request at /user')

  // Can access all parameters from req.body 
  //console.log('POST parameter received are: ',req.body['note']) 
  
  // Can access all parameters from req.body 
  //console.log(req.body);
  //console.log('POST parameter received are:', req.body)
  //console.log('Request body is : ', req.body)
  
  //var noteContent = '';
  if(req.body.note != null){
    var newNote = req.body.note;
    curNote = newNote;
  }
  else{
    //noteContent = "req.body.note is null."
    console.log('req.body is : ', req.body);
  }

  //var noteContent  = req.body.note;
  //console.log('req.body.note is : ', noteContent);

  //data = req.params.note;  
  //console.log('note:', data);

  //curNote = noteContent;

  //var note = req.params['note']
  //console.log(note);
  //res.send(note);
  
  // if we want to return JSON data
  // req.end() does NOT set header according to content of output
  //res.setHeader('Content-Type', 'application/json');
  //res.end(JSON.stringify({ note: curNote }));

  /*
  // req.send() will automatically set header according to content of output
  res.setHeader('Content-Type', 'application/json');
  res.send(JSON.stringify({ note: curNote }));
  // Content-Type in header will be set as "text/html; charset=utf-8"
  */

  // return saved note, only for double checking
  res.send(curNote);
})

app.get('/note', (req, res) =>{
/* -------------------------------------------------------------------------
EN: return the value of the Current Note
ES: Devolver el valor de la Nota actual
------------------------------------------------------------------------- */

  console.log('curNote = ', curNote);
  res.send(curNote);
})

app.delete('/note', (req, res) => {
/* -------------------------------------------------------------------------
EN: Clear value of the Current Note variable
ES: Borrar  el valor de la Nota actual
------------------------------------------------------------------------- */

  curNote = '';
  console.log('curNote = ', curNote);
  //res.send('Note has been cleared!');
  res.send(curNote);
  })


var notes = [];
var MAX_SIZE = 5;
var BLOCK_SIZE = 2;

app.post('/notes', (req, res) => {
/* -------------------------------------------------------------------------
Read the request BODY parameter 'note' and append it to the note list.
If input is empty string:
- doesn't append empty string
- and return empty string

ES: Read the request BODY parameter 'note' and append it to the note list. Empty string is accepted.
ES: Leer el parámetro BODY de la solicitud 'nota' y añádirlo a la lista de notas. Se acepta el texto vacío.
------------------------------------------------------------------------- */

  console.log('req.body.note is : ', noteContent);

  var noteContent  = '';
  if(req.body.note != null){
    console.log('Notes BEFORE adding new note:', notes);

    noteContent  = req.body.note;
    if (notes.length >= MAX_SIZE)
    {
      //notes = []; // in case we want to clear the list of notes when it reaches the max size
      
      const formattedDateHour = genDateHourStr();
      const historyFile = `./history/note_${formattedDateHour}.txt`;
      
      /*
      if(!fs.existsSync('./history')){
        fs.mkdirSync('./history', { recursive:true } );
      }
        
      for(var i=0; i<BLOCK_SIZE; i++){
        var oldNote = notes.shift(); // we only remove the oldest note
        fs.appendFileSync(historyFile, '\n' + oldNote);
      }
        */

      createDir('./history');
      var oldNotes = '';
      for(var i=0; i<BLOCK_SIZE; i++){
        var oldNote = notes.shift();
        oldNotes = '\n' + oldNote;        
      }
      fs.appendFileSync(historyFile, '\n' + oldNotes);
      // fs.appendFile(historyFile, '\n' + oldNotes);
    }
    notes.push(noteContent);

    console.log('Notes AFTER adding new note:', notes);

    res.send(noteContent);
  }
  else{
    res.status(400);
    res.end();
  }
  
/*
  if(noteContent.length > 0)
  {
    console.log('notes.length : ', notes.length);
    if (notes.length >= MAX_SIZE)
    {
      //notes = []; // in case we want to clear the list of notes when it reaches the max size
      notes.shift(); // we only remove the oldest note 
    }
    notes.push(noteContent);
  }
    */

  /*
  // if we want to return JSON data
  // req.end() does NOT set header according to content of output
  res.setHeader('Content-Type', 'application/json');
  res.send(JSON.stringify({
    noOfNote: notes.length, 
    MAX_SIZE: MAX_SIZE,
    lastNote: notes[notes.length-1],
    firstNote: notes[0]
  }));
  */

  /*
  if (noteContent.length > 0){
    res.send(notes[notes.length - 1]);
  }      
  else{
    res.send('');
  }
    */
})

app.get('/notes', (req, res) =>{
/* -------------------------------------------------------------------------
EN: Return the note list.
ES: Devolver la lista de notas.
------------------------------------------------------------------------- */

  //console.log('notes = ', notes);
  
  res.send(notes);

  /*
  if (notes.length > 0)
  {
    res.send(notes[notes.length - 1]);
  }      
  else{
    res.send('');
  } 
    */ 
})

app.delete('/notes', (req, res) => {
/* -------------------------------------------------------------------------
EN: Reset the note list. Return the size of the empty note list (should be 0)
ES: Restablecer la lista de notas. Devolver el tamaño de la lista de notas vacía (debe ser 0).
------------------------------------------------------------------------- */

  notes = [];
  //res.send(notes);
  //res.send(true);
  //res.send(notes.length);

  console.log("notes.length:", notes.length);
  res.send(String(notes.length));
  })

  /*
app.put('/user', (req, res) => {
    res.send('Got a PUT request at /user');
  })

app.delete('/user', (req, res) => {
res.send('Got a DELETE request at /user');
})
*/

app.listen(port, () => {
/* -------------------------------------------------------------------------
EN: listen to in the port
ES: Escuchar en el puerto
------------------------------------------------------------------------- */
    console.log(`Express.js app is listening on port ${port}`);
})
