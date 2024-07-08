using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GestionDeNotas.BusinessLogic
{
    // EN: A very simple Service simulator for testing the behavior of UI
    // ES: Un simulador de servicio muy simple para probar el comportamiento de la interfaz de usuario
    public class NoteServiceSimulator : INoteService
    {
        public bool IsAvailable { get; set; } = true;

        public async Task<bool> CheckServiceAvailable()
        {
            return IsAvailable;
        }

        private string _curNote = string.Empty;

        public async Task<string?> SaveNoteAsync(string note)
        {
            if (IsAvailable)
            {
                _curNote = note;
                return _curNote;
            }
            else
            {
                return null;
            }
        }

        public async Task<string?> ReadNoteAsync()
        {
            return IsAvailable ? _curNote : null;
        }

        public async Task<bool> DeleteNoteAsync()
        {
            if (IsAvailable)
            {
                _curNote = string.Empty;
                return true;
            }
            else
            {
                return false;
            }
        }

        private List<string> _notes = new List<string>();

        public async Task<string?> AppendNoteAsync(string note)
        {
            if (IsAvailable)
            {
                _notes.Add(note); // only for testing, ignore the max length of note list
                return _notes[_notes.Count - 1];
            }
            else
            {
                return null;
            }
        }

        public async Task<string[]?> ReadNotesAsync()
        {
            if (IsAvailable)
            {
                return _notes.ToArray();
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> ClearNotesAsync()
        {
            if (IsAvailable)
            {
                _notes = new List<string>();
                return true;
            }
            else
            {
                return false;
            }

        }

    }
}
