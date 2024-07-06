using GestionDeNotas.BusinessLogic;

namespace GestionDeNotas.BusinessLogic.Test
{
    [TestClass]
    public class NoteServiceTest
    {
        private static string _serviceBaseURL = "http://localhost:3000/";

        private static INoteService service; // = new NoteServiceWrapper(_serviceBaseURL);

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            service = new NoteServiceWrapper(_serviceBaseURL);
        }

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
        public async Task B0_SaveNoteAsync()
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
            {
                bool cleared = await service.ClearNotesAsync();
                Assert.IsTrue(cleared);
            }

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

        const int NOTE_LIST_MAX_SIZE = 5;

        [TestMethod]
        public async Task C0_1_AppendNoteAsync_outOfRange()
        {
            {
                bool cleared = await service.ClearNotesAsync();
                Assert.IsTrue(cleared);
            }

            for (int i = 0; i < NOTE_LIST_MAX_SIZE; i++)
            {
                string? savedNote = await service.AppendNoteAsync(i.ToString());
                Assert.AreEqual(savedNote, i.ToString());
            }
            {
                string[]? curNotes = await service.ReadNotesAsync();
                string[] expected = new string[NOTE_LIST_MAX_SIZE];
                for (int i = 0; i < NOTE_LIST_MAX_SIZE; i++)
                {
                    expected[i] = i.ToString();
                }
                CollectionAssert.AreEqual(curNotes, expected);
            }

            for (int i = 0; i < 3; i++)
            {
                string? savedNote = await service.AppendNoteAsync((NOTE_LIST_MAX_SIZE + i).ToString());
                Assert.AreEqual(savedNote, (NOTE_LIST_MAX_SIZE + i).ToString());
            }
            {
                string[]? curNotes = await service.ReadNotesAsync();
                CollectionAssert.AreEqual(curNotes, new string[] { "4", "5", "6", "7" });
            }
        }

        [TestMethod]
        public async Task C1_ReadNoteAsync()
        {
            bool cleared = await service.ClearNotesAsync();
            Assert.IsTrue(cleared);

            string[] notes = new string[] {
                "Samba No Pé",
                "Lambada",
                "",
                " Salsa ",
                "   ",
                "Brazil",
                "Bachata Dominicana!",
                "Chachachá"
            };

            // Current setting in server: MAX SIZE of Note list = 5, BLOCK_SIZE (for moving to history file) = 2
            for (int i = 0; i < notes.Length; i++)
            {
                string note = notes[i];
                string? savedNote = await service.AppendNoteAsync(note);
                Assert.AreEqual(savedNote, note);

                if (i == 3)
                {
                    string[]? curNotes = await service.ReadNotesAsync();
                    CollectionAssert.AreEqual(curNotes,
                        new string[] { "Samba No Pé", "Lambada", "", " Salsa " });
                }

                if (i == 5)
                {
                    string[]? curNotes = await service.ReadNotesAsync();
                    CollectionAssert.AreEqual(curNotes,
                        //new string[] { "Lambada", "", " Salsa ", "   ", "Brazil" });
                        new string[] { "", " Salsa ", "   ", "Brazil" });
                }
            }

            {
                string[]? curNotes = await service.ReadNotesAsync();
                CollectionAssert.AreEqual(curNotes,
                    //new string[] { "", " Salsa ", "   ", "Brazil", "Bachata Dominicana!" });
                    new string[] { "   ", "Brazil", "Bachata Dominicana!", "Chachachá" });
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