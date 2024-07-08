using GestionDeNotas.BusinessLogic;
using System.Runtime.InteropServices;

namespace GestionDeNotas.BusinessLogic.Test
{
    [TestClass]
    public class NoteServiceTest
    {
        private static string _serviceBaseURL = "http://localhost:3000/";

        private static INoteService service; // = new NoteServiceWrapper(_serviceBaseURL);

        private static Random rand = new Random();

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            service = new NoteServiceWrapper(_serviceBaseURL);
        }
        /*[ClassInitialize]
        public static void ClassInitialize()
        {
            service = new NoteServiceWrapper(_serviceBaseURL);
        }*/

        [TestMethod]
        public async Task A0_CheckServiceAvailable_On()
        {
            bool avaiblae = await service.CheckServiceAvailable();
            Assert.IsTrue(avaiblae);
        }

        [TestMethod]
        public async Task A1_CheckServiceAvailable_Off()
        {
            string wrongServiceBaseURL = "http://localhost:3001/";
            INoteService wrongNoteServiceWrapper = new NoteServiceWrapper(wrongServiceBaseURL);

            bool avaiblae = await wrongNoteServiceWrapper.CheckServiceAvailable();
            Assert.IsFalse(avaiblae);
        }

        [TestMethod]
        public async Task B0_0_SaveNoteAsync()
        {
            {
                string note = "Samba";
                string? savedNote = await service.SaveNoteAsync(note);
                Assert.AreEqual(savedNote.ToUpper(), "SAMBA");
            }
            {
                string note = "LAMBADA";
                string? savedNote = await service.SaveNoteAsync(note);
                Assert.AreEqual(savedNote.ToUpper(), note);
            }
            {
                string note = "SalsA";
                string? savedNote = await service.SaveNoteAsync(note);
                Assert.AreEqual(savedNote, "SalsA");
            }

            {
                string note = string.Empty;
                string? savedNote = await service.SaveNoteAsync(note);

                string? curNote = await service.ReadNoteAsync();
                Assert.AreEqual(curNote, "");
            }

            {
                string note = "   ";
                string? savedNote = await service.SaveNoteAsync(note);

                string? curNote = await service.ReadNoteAsync();
                Assert.AreEqual(curNote, "   ");
            }

            // long loop
            for (int i = 0; i < 100; i++)
            {
                string note = rand.NextDouble().ToString();
                string? savedNote = await service.SaveNoteAsync(note);
                Assert.AreEqual(savedNote, note);
            }
        }

        [TestMethod]
        public async Task B0_1_SaveNoteAsync_SpecialCharacters()
        {
            string note = "¡Hola!" + Environment.NewLine + "  ¿Mañana?";

            string? savedNote = await service.SaveNoteAsync(note);
            Assert.AreEqual(savedNote, note);

            string? curNote = await service.ReadNoteAsync();
            Assert.AreEqual(curNote, note);
        }

        [TestMethod]
        public async Task B0_2_SaveNoteAsync_LongText()
        {
            // TODO need to implement --> will FAILE
            Assert.AreEqual("Very Long Text", "Long Text");
        }

        [TestMethod]
        public async Task B0_3_SaveNoteAsync_NonLatinText()
        {
            // some content of like Japanese, Chinese...
            string japaneseText = @"こんにちは、友人。
お元気ですか？ お互い元気でいられるといいですね。
よろしくお願いいたします。";

            //Assert.AreEqual("Non Unicode Text", "Unicode Text");

            string? savedNote = await service.SaveNoteAsync(japaneseText);
            Assert.AreEqual(savedNote, japaneseText);

            string? curNote = await service.ReadNoteAsync();
            Assert.AreEqual(curNote, japaneseText);
        }

        [TestMethod]
        public async Task B1_0_ReadNoteAsync_simple()
        {
                string? curNote = await service.ReadNoteAsync();
                Assert.IsNotNull(curNote);
        }

        [TestMethod]
        public async Task B1_1_ReadNoteAsync_complex()
        {
            {
                // expect empty
                bool deleted = await service.DeleteNoteAsync();
                Assert.IsTrue(deleted);

                string? curNote = await service.ReadNoteAsync();
                Assert.AreEqual(curNote, string.Empty);
            }

            {
                // expect a string
                string note = "LAMBADA";
                string? savedNote = await service.SaveNoteAsync(note);
                Assert.AreEqual(savedNote.ToUpper(), "LAMBADA");

                string? curNote = await service.ReadNoteAsync();
                Assert.AreEqual(curNote, "LAMBADA");
            }

            // multiple times
            for (int i = 0; i < 10; i++)
            {
                string note = rand.NextDouble().ToString();
                string? savedNote = await service.SaveNoteAsync(note);
                Assert.AreEqual(savedNote, note);

                string? curNote = await service.ReadNoteAsync();
                Assert.AreEqual(curNote, note);
            }
        }

        [TestMethod]
        public async Task B2_0_DeleteNoteAsync_simple()
        {
            string? curNote = await service.ReadNoteAsync();
            Assert.IsNotNull(curNote);
        }

        [TestMethod]
        public async Task B2_1_DeleteNoteAsync_complex()
        {
            {
                string note = "LAMBADA";
                string? savedNote = await service.SaveNoteAsync(note);
                Assert.AreEqual(savedNote.ToUpper(), "LAMBADA");

                bool deleted = await service.DeleteNoteAsync();
                Assert.IsTrue(deleted);

                string? curNote = await service.ReadNoteAsync();
                Assert.AreEqual(curNote, "");
            }
        }

        [TestMethod]
        public async Task C0_0_AppendNoteAsync()
        {
            bool cleared = await service.ClearNotesAsync();
            Assert.IsTrue(cleared);

            {
                string note = "Samba No Pé";
                string? savedNote = await service.AppendNoteAsync(note);
                Assert.AreEqual(savedNote, note);
            }
            {
                string[]? curNotes = await service.ReadNotesAsync();
                CollectionAssert.AreEqual(curNotes, new string[] { "Samba No Pé" });
            }

            {
                string note = "Caballero y Senorita";
                string? savedNote = await service.AppendNoteAsync(note);
                Assert.AreEqual(savedNote, note);
            }
            {
                string[]? curNotes = await service.ReadNotesAsync();
                CollectionAssert.AreEqual(curNotes, new string[] { "Samba No Pé", "Caballero y Senorita" });
            }
        }

        [TestMethod]
        public async Task C0_1_AppendNoteAsync_SpecialCharacters()
        {
            string note = @"¡Hola! 

¿Mañana?";
            string? savedNote = await service.AppendNoteAsync(note);
            Assert.AreEqual(savedNote, @"¡Hola! 

¿Mañana?");

            string[]? curNotes = await service.ReadNotesAsync();
            Assert.AreEqual(curNotes[curNotes.Length - 1], @"¡Hola! 

¿Mañana?");
        }

        [TestMethod]
        public async Task C0_2_AppendNoteAsync_LongText()
        {
            // TODO need to implement --> will FAIL
            Assert.AreEqual("Very Long Text", "Long Text");
        }

        [TestMethod]
        public async Task C0_3_AppendNoteAsync_NonLatinText()
        {
            // some content of like Chinese, Japanese...
            string japaneseText = @"こんにちは、友人。
お元気ですか？ お互い元気でいられるといいですね。
よろしくお願いいたします。";

            string? savedNote = await service.AppendNoteAsync(japaneseText);
            Assert.AreEqual(savedNote, japaneseText);

            string[]? curNotes = await service.ReadNotesAsync();
            Assert.AreEqual(curNotes[curNotes.Length - 1], japaneseText);
        }

        [TestMethod]
        public async Task C0_1_AppendNoteAsync_outOfRange()
        {
            // test with long loop
            const int NO_OF_TEST = 1000;
            
            for (int i = 0; i < NO_OF_TEST; i++)
            {
                string note = rand.NextInt64().ToString();
                string? savedNote = await service.AppendNoteAsync(note);
                Assert.AreEqual(savedNote, note);
            }

            // check last value saved in server
            for (int i = 0; i < NO_OF_TEST; i++)
            {
                string note = rand.NextInt64().ToString();
                string? savedNote = await service.AppendNoteAsync(note);
                Assert.AreEqual(savedNote, note);

                string[]? curNotes = await service.ReadNotesAsync();
                Assert.AreEqual(curNotes[curNotes.Length - 1], note);
            }
        }

        [TestMethod]
        public async Task C1_ReadNotesAsync()
        {
            bool cleared = await service.ClearNotesAsync();
            Assert.IsTrue(cleared);

            const int NO_OF_TEST = 50;

            List<string> noteList = new List<string>();
            for (int i = 0; i < NO_OF_TEST; i++)
            {
                noteList.Add(i.ToString());
            }

            // check each values in server
            string[] notesInLocal = noteList.ToArray();
            for (int i = 0; i < NO_OF_TEST; i++)
            {
                string note = notesInLocal[i];
                string? savedNote = await service.AppendNoteAsync(note);
                Assert.AreEqual(savedNote, note);

                string[]? notesInServer = await service.ReadNotesAsync();
                for (int j = 0; j <= i && j < notesInServer.Length; j++)
                {
                    string noteInLocal = notesInLocal[i - j];
                    string noteInServer = notesInServer[notesInServer.Length - 1 - j];
                    Assert.AreEqual(noteInServer, noteInServer);
                }
            }
        }

        [TestMethod]
        public async Task C2_ClearNotesAsync()
        {
            {
                bool cleared = await service.ClearNotesAsync();
                Assert.IsTrue(cleared);

                string[] notes = new string[] { "Samba No Pé", "Lambada", " Salsa ", "Brazil", "Bachata Dominicana!" };
                foreach (string note in notes)
                {
                    await service.AppendNoteAsync(note);
                }
            }

            {
                bool cleared = await service.ClearNotesAsync();
                Assert.IsTrue(cleared);

                string[]? curNotes = await service.ReadNotesAsync();
                CollectionAssert.AreEqual(curNotes, new string[] { });
            }
        }
    }
}