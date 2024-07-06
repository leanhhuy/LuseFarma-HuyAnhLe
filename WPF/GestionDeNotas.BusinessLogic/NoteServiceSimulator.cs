using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionDeNotas.BusinessLogic
{
    public class NoteServiceSimulator : INoteService
    {
        public async Task<bool> CheckServiceAvailable()
        {
            return true;
        }

        private string _curNote = string.Empty;

        public async Task<string?> SaveNoteAsync(string note)
        {
            _curNote = note;
            return _curNote;
        }

        public async Task<string?> ReadNoteAsync()
        {
            return _curNote;
        }

        public async Task<bool> DeleteNoteAsync()
        {
            _curNote = string.Empty;
            return true;
        }

        private List<string> _notes = new List<string>();

        public async Task<string?> AppendNoteAsync(string note)
        {
            _notes.Add(note); // only for testing, ignore the max length of note list
            return _notes[_notes.Count - 1];
        }

        public async Task<string[]?> ReadNotesAsync()
        {
            return _notes.ToArray();
        }

        public async Task<bool> ClearNotesAsync()
        {
            _notes = new List<string>();
            return true;
        }


    }
}
