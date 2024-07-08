const port = 3000;

const express = require('express');
const app = express();

const fs = require('node:fs');
const path = require('node:path');

// Using express.urlencoded middleware
// app.use(express.json()) // for parsing application/json. NOT USE NOW.
app.use(express.urlencoded({ extended: true })) // for parsing application/x-www-form-urlencoded

app.use((req, res, next) => {
/* -------------------------------------------------------------------------
// In este proyecto, creamos una función de Middleware para logging
// https://expressjs.com/en/guide/writing-middleware.html
// Otra opción la biblioteca: https://www.npmjs.com/package/express-requests-logger
------------------------------------------------------------------------- */

  // Use TRY - CATCH to avoid error that can crash the API
  try {
    const now = new Date();
    console.log(now);
    
    const formattedDateHour = genDateHourStr();
    const logFile = `./log/log_${formattedDateHour}.txt`;

    console.log('BEFORE processing request: log to file', logFile);

    createDir('./log');
    
    // https://nodejs.org/en/learn/manipulating-files/writing-files-with-nodejs

    // if we need the absolute path of the log file in server
    // var absolutePath = path.resolve(logFile);

    fs.appendFileSync(logFile, '\n' + now.toISOString());

    // Documents about the request
    // https://expressjs.com/en/api.html#req.baseUrl
    // https://expressjs.com/en/api.html#req.originalUrl        
    const protocol = req.protocol;

    //const host = req.hostname; // hostname without port
    const host = req.get('Host') // hostname withport
    //console.log('Hostname with port: ', host);

    //const query = req.query; // get URL query parameters
    const url = req.originalUrl; // with query    
    //console.log('URL with query: ', url);

    const fullUrl = `${protocol}://${host}${url}`;
    const medthod = req.method;
    console.log(medthod + ": " + fullUrl);

    fs.appendFileSync(logFile, '\n' + medthod + ": " + fullUrl);

    /*for(var key in req.body){
        var val = req.body[key];
        console.log(key, ":", val);
    }*/
    const data = JSON.stringify(req.body);
    console.log('Request body to JSON data: ', data);
    fs.appendFileSync(logFile, '\n' + data);
  } catch (err) {
    console.error(err);
  }
  
  next();
  
  // Because this code is simple, we can remove TRY - CATCH  
  /* try { */
    console.log('AFTER: request has been processed.');
  /* } catch (err) {
    console.error(err);
  } */
});

function genDateHourStr() {
  const now = new Date();
  const year = now.getFullYear();

  // Be carefull: from 0 to 11, NOT from 1 to 12
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

app.get('/', (req, res) => {
  /* -------------------------------------------------------------------------
  EN: Root URL: for checking if service is available
  ES: Root URL: para comprobar si el servicio está disponible
  ------------------------------------------------------------------------- */
    res.send('Hello World!'); 
  })

/* -------------------------------- Parte OBLIGATORIO ----------------------------------------- */
var curNote = '';

app.post('/note', (req, res) => {
/* -------------------------------------------------------------------------
ES: Read the request BODY parameter 'note' and save it to the Current Note variable. Empty string is accepted.
ES: Leer el parámetro 'note' del BODY del request y guardarlo a la variable Nota actual. Se acepta el texto vacío.
------------------------------------------------------------------------- */
  if(req.body.note != null){
    curNote = req.body.note
    // return saved note, only for double checking
    res.send(curNote);
  }
  else{
    res.status(400);
    res.end();
  }
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
  res.send(curNote);
})
/* -------------------------------- Fin de parte OBLIGATORIO ----------------------------------------- */

/* -------------------------------- Parte OPCIONAL ----------------------------------------- */
var noteList = [];
var MAX_SIZE = 5;
var BLOCK_SIZE = 2;

app.post('/notes', (req, res) => {
/* -------------------------------------------------------------------------
ES: Read the request BODY parameter 'note' and append it to the note list. Empty string is accepted.
ES: Leer el parámetro 'note' del BODY del request y añádirlo a la lista de notas. Se acepta el texto vacío.
------------------------------------------------------------------------- */
  if(req.body.note != null){
    console.log('Notes BEFORE adding new note:', noteList);

    var noteContent  = req.body.note;
    console.log('req.body.note is : ', noteContent);

    // cut a block of old notes to history file
    if (noteList.length >= MAX_SIZE)
    { 
      const formattedDateHour = genDateHourStr();
      const historyFile = `./history/note_${formattedDateHour}.txt`;
      
      createDir('./history');
      var oldNotes = '';
      for(var i=0; i<BLOCK_SIZE; i++){
        var oldNote = noteList.shift();
        oldNotes = '\n' + oldNote;        
      }

      fs.appendFileSync(historyFile, '\n' + oldNotes);
    }

    noteList.push(noteContent);

    console.log('Notes AFTER adding new note:', noteList);

    res.send(noteContent);
  }
  else{
    res.status(400);
    res.end();
  }
})

app.get('/notes', (req, res) =>{
/* -------------------------------------------------------------------------
EN: Return the note list.
ES: Devolver la lista de notas.
------------------------------------------------------------------------- */
  console.log('notes = ', noteList);    
  res.send(noteList);
})

app.delete('/notes', (req, res) => {
/* -------------------------------------------------------------------------
EN: Reset the note list. Return the size of the empty note list (should be 0)
ES: Restablecer la lista de notas. Devolver el tamaño de la lista de notas vacía (debe ser 0).
------------------------------------------------------------------------- */

  noteList = [];
  console.log("notes =", noteList);
  res.send(String(noteList.length));
})
/* -------------------------------- Fin de parte OPCIONAL ----------------------------------------- */

app.listen(port, () => {
  /* -------------------------------------------------------------------------
  EN: listen at the port 3000. 
  ES: Escuchar en el puerto 3000
  ------------------------------------------------------------------------- */
        console.log(`Express.js app is listening on port ${port}`);
  })  
