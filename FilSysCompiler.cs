using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using filesys.System;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace filesys.GUI
{
    public class FilSysStudioWindow : BaseWindow
    {
        private string sourcePath = @"0:\apps\main.fsrc";
        private string bytecodePath = @"0:\apps\main.fbin";

        public static WindowManager WindowMgr;

        private bool editorFocused = false;
        private bool lastClick = false;

        private string editorText =
@"app ""Mon Application""

func main
print ""Application lancee !""
end

func hello
print ""Bonjour depuis un bouton !""
save ""0:\apps\bouton.txt"" ""Le bouton a fonctionne""
end

button ""Dire bonjour"" call hello
button ""Effacer sortie"" call clearOutput

func clearOutput
clear
end";

        private string output = "Pret.";

        private static Pen penWhite = new Pen(Color.White);
        private static Pen penGray = new Pen(Color.Gray);
        private static Pen penGreen = new Pen(Color.LightGreen);
        private static Pen penButton = new Pen(Color.FromArgb(70, 70, 70));
        private static Pen penBox = new Pen(Color.FromArgb(20, 20, 20));
        private static Pen penRuntimeButton = new Pen(Color.FromArgb(60, 60, 90));

        public FilSysStudioWindow(int x, int y)
            : base("FilSys Studio", x, y, 680, 460)
        {
            try
            {
                if (!Directory.Exists(@"0:\apps\"))
                    Directory.CreateDirectory(@"0:\apps\");
            }
            catch
            {
                output = "Erreur : disque non initialise.";
            }
        }

        public override void Update()
        {
            base.Update();

            if (IsClosed || IsMinimized)
                return;

            int mx = (int)MouseManager.X;
            int my = (int)MouseManager.Y;
            bool click = MouseManager.MouseState == MouseState.Left;

            if (click && !lastClick)
            {
                if (IsButton(mx, my, X + 10, Y + 35, 90, 24))
                    CompileRun();
                else if (IsButton(mx, my, X + 110, Y + 35, 90, 24))
                    SaveSource();
                else if (IsButton(mx, my, X + 210, Y + 35, 90, 24))
                    LoadSource();
                else if (IsButton(mx, my, X + 310, Y + 35, 130, 24))
                    CreateAppWindow();

                editorFocused = IsButton(mx, my, X + 10, Y + 93, Width - 20, 220);
            }

            if (editorFocused)
                UpdateEditorKeyboard();

            lastClick = click;
        }

        public override void Draw(Canvas canvas)
        {
            base.Draw(canvas);

            if (IsClosed || IsMinimized)
                return;

            DrawButton(canvas, X + 10, Y + 35, 90, 24, "Run");
            DrawButton(canvas, X + 110, Y + 35, 90, 24, "Save");
            DrawButton(canvas, X + 210, Y + 35, 90, 24, "Load");
            DrawButton(canvas, X + 310, Y + 35, 130, 24, "Create App");

            canvas.DrawString("SOURCE", PCScreenFont.Default, penWhite, X + 10, Y + 75);

            DrawBox(
                canvas,
                X + 10,
                Y + 93,
                Width - 20,
                220,
                editorFocused ? editorText + "_" : editorText,
                penGreen);

            canvas.DrawString("SORTIE", PCScreenFont.Default, penWhite, X + 10, Y + 325);

            DrawBox(
                canvas,
                X + 10,
                Y + 343,
                Width - 20,
                Height - 355,
                output,
                penWhite);
        }

        private void UpdateEditorKeyboard()
        {
            KeyEvent key;

            while (KeyboardManager.TryReadKey(out key))
            {
                if (key.Key == ConsoleKeyEx.Backspace)
                {
                    if (editorText.Length > 0)
                        editorText = editorText.Substring(0, editorText.Length - 1);
                }
                else if (key.Key == ConsoleKeyEx.Enter)
                {
                    editorText += "\n";
                }
                else if (key.Key == ConsoleKeyEx.Tab)
                {
                    editorText += "    ";
                }
                else if (key.KeyChar != '\0')
                {
                    editorText += key.KeyChar;
                }

                if (editorText.Length > 6000)
                    editorText = editorText.Substring(editorText.Length - 6000);
            }
        }

        private void CompileRun()
        {
            try
            {
                FilSysCompiledProgram program = FilSysRealCompiler.Compile(editorText);
                FilSysRealCompiler.SaveBytecode(program, bytecodePath);

                FilSysVirtualMachine vm = new FilSysVirtualMachine(program);
                output = vm.RunFunction("main");

                LimitOutput();
            }
            catch (Exception ex)
            {
                output = "Erreur : " + ex.Message;
            }
        }

        private void CreateAppWindow()
        {
            try
            {
                FilSysCompiledProgram program = FilSysRealCompiler.Compile(editorText);
                FilSysRealCompiler.SaveBytecode(program, bytecodePath);

                if (WindowMgr != null)
                {
                    WindowMgr.Add(new FilSysRuntimeAppWindow(program, X + 40, Y + 40));
                    output = "Application creee : " + program.AppName;
                }
                else
                {
                    output = "Erreur : WindowMgr est null.";
                }
            }
            catch (Exception ex)
            {
                output = "Erreur creation app : " + ex.Message;
            }
        }

        private void SaveSource()
        {
            try
            {
                File.WriteAllText(sourcePath, editorText);
                output = "Source sauvegarde : " + sourcePath;
            }
            catch (Exception ex)
            {
                output = "Erreur sauvegarde : " + ex.Message;
            }
        }

        private void LoadSource()
        {
            try
            {
                if (!File.Exists(sourcePath))
                {
                    output = "Source introuvable.";
                    return;
                }

                editorText = File.ReadAllText(sourcePath);

                if (editorText.Length > 6000)
                    editorText = editorText.Substring(0, 6000);

                output = "Source charge.";
            }
            catch (Exception ex)
            {
                output = "Erreur chargement : " + ex.Message;
            }
        }

        private void LimitOutput()
        {
            if (output.Length > 3000)
                output = output.Substring(output.Length - 3000);
        }

        private bool IsButton(int mx, int my, int bx, int by, int bw, int bh)
        {
            return mx >= bx && mx <= bx + bw && my >= by && my <= by + bh;
        }

        private void DrawButton(Canvas canvas, int x, int y, int w, int h, string text)
        {
            canvas.DrawFilledRectangle(penButton, x, y, w, h);
            canvas.DrawRectangle(penWhite, x, y, w, h);
            canvas.DrawString(text, PCScreenFont.Default, penWhite, x + 8, y + 7);
        }

        private void DrawBox(Canvas canvas, int x, int y, int w, int h, string text, Pen textPen)
        {
            canvas.DrawFilledRectangle(penBox, x, y, w, h);
            canvas.DrawRectangle(penGray, x, y, w, h);

            string[] lines = text.Replace("\r", "").Split('\n');

            int yy = y + 6;
            int maxLines = h / 14;

            int start = 0;

            if (lines.Length > maxLines)
                start = lines.Length - maxLines;

            for (int i = start; i < lines.Length; i++)
            {
                if (yy > y + h - 14)
                    break;

                string line = lines[i];

                if (line.Length > 90)
                    line = line.Substring(0, 90);

                canvas.DrawString(line, PCScreenFont.Default, textPen, x + 6, yy);
                yy += 14;
            }
        }
    }

    public class FilSysRuntimeAppWindow : BaseWindow
    {
        private FilSysCompiledProgram program;
        private FilSysVirtualMachine vm;

        private string output = "";
        private bool lastClick = false;

        private static Pen penWhite = new Pen(Color.White);
        private static Pen penGray = new Pen(Color.Gray);
        private static Pen penGreen = new Pen(Color.LightGreen);
        private static Pen penBox = new Pen(Color.FromArgb(20, 20, 20));
        private static Pen penButton = new Pen(Color.FromArgb(60, 60, 90));

        public FilSysRuntimeAppWindow(FilSysCompiledProgram compiledProgram, int x, int y)
            : base(compiledProgram.AppName, x, y, 420, 320)
        {
            program = compiledProgram;
            vm = new FilSysVirtualMachine(program);
            output = vm.RunFunction("main");
        }

        public override void Update()
        {
            base.Update();

            if (IsClosed || IsMinimized)
                return;

            int mx = (int)MouseManager.X;
            int my = (int)MouseManager.Y;
            bool click = MouseManager.MouseState == MouseState.Left;

            if (click && !lastClick)
            {
                for (int i = 0; i < program.Buttons.Count; i++)
                {
                    int bx = X + 10;
                    int by = Y + 40 + i * 32;

                    if (mx >= bx && mx <= bx + 180 && my >= by && my <= by + 24)
                    {
                        output += vm.RunFunction(program.Buttons[i].FunctionName);

                        if (output.Length > 3000)
                            output = output.Substring(output.Length - 3000);
                    }
                }
            }

            lastClick = click;
        }

        public override void Draw(Canvas canvas)
        {
            base.Draw(canvas);

            if (IsClosed || IsMinimized)
                return;

            for (int i = 0; i < program.Buttons.Count; i++)
            {
                int bx = X + 10;
                int by = Y + 40 + i * 32;

                canvas.DrawFilledRectangle(penButton, bx, by, 180, 24);
                canvas.DrawRectangle(penWhite, bx, by, 180, 24);
                canvas.DrawString(program.Buttons[i].Text, PCScreenFont.Default, penWhite, bx + 8, by + 7);
            }

            canvas.DrawString("SORTIE", PCScreenFont.Default, penWhite, X + 210, Y + 40);
            DrawOutput(canvas);
        }

        private void DrawOutput(Canvas canvas)
        {
            int x = X + 210;
            int y = Y + 60;
            int w = Width - 220;
            int h = Height - 70;

            canvas.DrawFilledRectangle(penBox, x, y, w, h);
            canvas.DrawRectangle(penGray, x, y, w, h);

            string[] lines = output.Replace("\r", "").Split('\n');

            int yy = y + 6;
            int maxLines = h / 14;

            int start = 0;

            if (lines.Length > maxLines)
                start = lines.Length - maxLines;

            for (int i = start; i < lines.Length; i++)
            {
                if (yy > y + h - 14)
                    break;

                string line = lines[i];

                if (line.Length > 25)
                    line = line.Substring(0, 25);

                canvas.DrawString(line, PCScreenFont.Default, penGreen, x + 6, yy);
                yy += 14;
            }
        }
    }
}

namespace filesys.System
{
    public enum FilSysOpCode
    {
        PrintText,
        PrintVar,
        SetVar,
        SaveFile,
        LoadFile,
        MakeDir,
        Clear,
        Call
    }

    public class FilSysButton
    {
        public string Text;
        public string FunctionName;

        public FilSysButton(string text, string functionName)
        {
            Text = text;
            FunctionName = functionName;
        }
    }

    public class FilSysInstruction
    {
        public FilSysOpCode Code;
        public string A;
        public string B;

        public FilSysInstruction(FilSysOpCode code, string a, string b)
        {
            Code = code;
            A = a;
            B = b;
        }
    }

    public class FilSysFunction
    {
        public string Name;
        public List<FilSysInstruction> Instructions = new List<FilSysInstruction>();

        public FilSysFunction(string name)
        {
            Name = name;
        }
    }

    public class FilSysCompiledProgram
    {
        public string AppName = "Application";
        public Dictionary<string, FilSysFunction> Functions = new Dictionary<string, FilSysFunction>();
        public List<FilSysButton> Buttons = new List<FilSysButton>();
    }

    public class FilSysRealCompiler
    {
        public static FilSysCompiledProgram Compile(string source)
        {
            FilSysCompiledProgram program = new FilSysCompiledProgram();

            string[] lines = source.Replace("\r", "").Split('\n');
            FilSysFunction currentFunction = null;

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();

                if (line == "" || line.StartsWith("//"))
                    continue;

                if (line.StartsWith("app "))
                {
                    program.AppName = UnString(line.Substring(4).Trim());
                    continue;
                }

                if (line.StartsWith("func "))
                {
                    string funcName = line.Substring(5).Trim();

                    if (program.Functions.ContainsKey(funcName))
                        throw new Exception("Fonction deja existante : " + funcName);

                    currentFunction = new FilSysFunction(funcName);
                    program.Functions.Add(funcName, currentFunction);
                    continue;
                }

                if (line == "end")
                {
                    currentFunction = null;
                    continue;
                }

                if (line.StartsWith("button "))
                {
                    string[] args = ParseButton(line);
                    program.Buttons.Add(new FilSysButton(args[0], args[1]));
                    continue;
                }

                if (currentFunction == null)
                    throw new Exception("Ligne " + (i + 1) + " hors fonction : " + line);

                CompileLine(currentFunction, line, i + 1);
            }

            if (!program.Functions.ContainsKey("main"))
                throw new Exception("Il manque la fonction main.");

            return program;
        }

        private static void CompileLine(FilSysFunction func, string line, int lineNumber)
        {
            if (line.StartsWith("print "))
            {
                string value = line.Substring(6).Trim();

                if (IsString(value))
                    func.Instructions.Add(new FilSysInstruction(FilSysOpCode.PrintText, UnString(value), ""));
                else
                    func.Instructions.Add(new FilSysInstruction(FilSysOpCode.PrintVar, value, ""));

                return;
            }

            if (line.StartsWith("let "))
            {
                string code = line.Substring(4);
                int eq = code.IndexOf('=');

                if (eq == -1)
                    throw new Exception("let invalide ligne " + lineNumber);

                string name = code.Substring(0, eq).Trim();
                string value = code.Substring(eq + 1).Trim();

                if (IsString(value))
                    value = UnString(value);

                func.Instructions.Add(new FilSysInstruction(FilSysOpCode.SetVar, name, value));
                return;
            }

            if (line.StartsWith("call "))
            {
                string functionName = line.Substring(5).Trim();
                func.Instructions.Add(new FilSysInstruction(FilSysOpCode.Call, functionName, ""));
                return;
            }

            if (line.StartsWith("save "))
            {
                string[] args = ReadTwoStrings(line.Substring(5));
                func.Instructions.Add(new FilSysInstruction(FilSysOpCode.SaveFile, args[0], args[1]));
                return;
            }

            if (line.StartsWith("load "))
            {
                string path = UnString(line.Substring(5).Trim());
                func.Instructions.Add(new FilSysInstruction(FilSysOpCode.LoadFile, path, ""));
                return;
            }

            if (line.StartsWith("mkdir "))
            {
                string path = UnString(line.Substring(6).Trim());
                func.Instructions.Add(new FilSysInstruction(FilSysOpCode.MakeDir, path, ""));
                return;
            }

            if (line == "clear")
            {
                func.Instructions.Add(new FilSysInstruction(FilSysOpCode.Clear, "", ""));
                return;
            }

            throw new Exception("Commande inconnue ligne " + lineNumber + " : " + line);
        }

        public static void SaveBytecode(FilSysCompiledProgram program, string path)
        {
            string data = "APP|" + Escape(program.AppName) + "\n";

            foreach (FilSysButton btn in program.Buttons)
                data += "BTN|" + Escape(btn.Text) + "|" + Escape(btn.FunctionName) + "\n";

            foreach (FilSysFunction func in program.Functions.Values)
            {
                data += "FUNC|" + Escape(func.Name) + "\n";

                foreach (FilSysInstruction ins in func.Instructions)
                    data += "INS|" + ((int)ins.Code) + "|" + Escape(ins.A) + "|" + Escape(ins.B) + "\n";

                data += "END\n";
            }

            File.WriteAllText(path, data);
        }

        private static string[] ParseButton(string line)
        {
            string text = ReadFirstString(line.Substring(7));
            int callIndex = line.IndexOf(" call ");

            if (callIndex == -1)
                throw new Exception("Bouton invalide. Exemple : button \"OK\" call hello");

            string functionName = line.Substring(callIndex + 6).Trim();

            return new string[] { text, functionName };
        }

        private static bool IsString(string value)
        {
            value = value.Trim();
            return value.StartsWith("\"") && value.EndsWith("\"");
        }

        private static string UnString(string value)
        {
            value = value.Trim();

            if (IsString(value))
                return value.Substring(1, value.Length - 2);

            return value;
        }

        private static string ReadFirstString(string text)
        {
            bool inside = false;
            string current = "";

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                if (c == '"')
                {
                    if (inside)
                        return current;

                    inside = true;
                }
                else if (inside)
                {
                    current += c;
                }
            }

            throw new Exception("Texte entre guillemets attendu.");
        }

        private static string[] ReadTwoStrings(string text)
        {
            List<string> list = new List<string>();
            bool inside = false;
            string current = "";

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                if (c == '"')
                {
                    if (inside)
                    {
                        list.Add(current);
                        current = "";
                        inside = false;
                    }
                    else
                    {
                        inside = true;
                    }
                }
                else if (inside)
                {
                    current += c;
                }
            }

            if (list.Count < 2)
                throw new Exception("Il faut deux textes entre guillemets.");

            return list.ToArray();
        }

        private static string Escape(string s)
        {
            if (s == null)
                return "";

            return s.Replace("\\", "\\\\").Replace("|", "\\p").Replace("\n", "\\n");
        }
    }

    public class FilSysVirtualMachine
    {
        private FilSysCompiledProgram program;
        private Dictionary<string, string> variables = new Dictionary<string, string>();
        private int callDepth = 0;

        public FilSysVirtualMachine(FilSysCompiledProgram compiledProgram)
        {
            program = compiledProgram;
        }

        public string RunFunction(string functionName)
        {
            if (!program.Functions.ContainsKey(functionName))
                return "Fonction inconnue : " + functionName + "\n";

            if (callDepth > 16)
                return "Erreur : trop d'appels de fonctions.\n";

            callDepth++;

            string output = "";
            FilSysFunction func = program.Functions[functionName];

            for (int i = 0; i < func.Instructions.Count; i++)
            {
                FilSysInstruction ins = func.Instructions[i];

                if (ins.Code == FilSysOpCode.PrintText)
                {
                    output += ins.A + "\n";
                }
                else if (ins.Code == FilSysOpCode.PrintVar)
                {
                    if (variables.ContainsKey(ins.A))
                        output += variables[ins.A] + "\n";
                    else
                        output += "Variable inconnue : " + ins.A + "\n";
                }
                else if (ins.Code == FilSysOpCode.SetVar)
                {
                    variables[ins.A] = ins.B;
                }
                else if (ins.Code == FilSysOpCode.Call)
                {
                    output += RunFunction(ins.A);
                }
                else if (ins.Code == FilSysOpCode.SaveFile)
                {
                    File.WriteAllText(ins.A, ins.B);
                    output += "Fichier cree : " + ins.A + "\n";
                }
                else if (ins.Code == FilSysOpCode.LoadFile)
                {
                    if (File.Exists(ins.A))
                        output += File.ReadAllText(ins.A) + "\n";
                    else
                        output += "Fichier introuvable : " + ins.A + "\n";
                }
                else if (ins.Code == FilSysOpCode.MakeDir)
                {
                    if (!Directory.Exists(ins.A))
                        Directory.CreateDirectory(ins.A);

                    output += "Dossier cree : " + ins.A + "\n";
                }
                else if (ins.Code == FilSysOpCode.Clear)
                {
                    output = "";
                }

                if (output.Length > 3000)
                    output = output.Substring(output.Length - 3000);
            }

            callDepth--;
            return output;
        }
    }
}