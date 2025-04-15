using Spectre.Console;
using System.Text.Json;

public class Program{
    public static readonly string PATH = Environment.CurrentDirectory + "/tareas.todo";
    public static bool err = false;
    public static void Main()
    {
        AnsiConsole.MarkupLine("[yellow bold]¡Bienvenido![/]");
        ToDo td = new ToDo();
        if(File.Exists(PATH)){
            AnsiConsole.Markup("[blue bold]Se ha detectado una lista existente. Cargando...   [/]");
            td.Load(File.Open(PATH, FileMode.Open));
            AnsiConsole.MarkupLine("[green bold]¡Hecho![/]");
        }
        Thread.Sleep(1000);
        bool exit = false;
        while(!exit){
            AnsiConsole.Clear();
            if(err){
                AnsiConsole.Write(
                    new Panel("[bold red]Comando desconocido. Disponibles: Nuevo, Borrar, Guardar, Salir[/]")
                    .Border(BoxBorder.Double)
                    .BorderColor(Color.Red)
                    .Header("[bold red]Error[/]")
                );
                err = false;
            }
            PrintList(td);
            string command = AnsiConsole.Ask<string>("Introduce un comando: ");
            switch(command.ToLower()){
                case "nuevo":
                    Nuevo(td);
                    break;
                case "borrar":
                    Borrar(td);
                    break;
                case "guardar":
                    Guardar(td);
                    break;
                case "salir":
                    AnsiConsole.Clear();
                    AnsiConsole.Write(new Panel("[red bold]Confirmar operación[/]").Border(BoxBorder.Heavy).BorderColor(Color.Yellow).Header("[red bold]Advertencia[/]"));
                    if(AnsiConsole.Confirm("[black bold on red]¿Seguro que quieres salir?[/]", false))
                        exit = true;
                    break;
                default:
                    err = true;
                    break;
            }
        }
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[yellow bold]Adiós[/]");
    }

    public static void Nuevo(ToDo todo){
        string name = AnsiConsole.Ask<string>("[bold]Nombre: [/]");
        string fechahora = AnsiConsole.Ask<string>("[bold]Fecha y hora [[Formato dd/mm/aaaa hh:mm:ss]]: [/]", DateTime.Now.ToString());
        DateTime dt;
        if(!DateTime.TryParse(fechahora, out dt))
        {
            AnsiConsole.MarkupLine("[red bold]Formato incorrecto[/]");
            Thread.Sleep(2000);
        }
        todo.Add(new Tarea(name, dt));
        AnsiConsole.MarkupLine("[blue bold]Añadido[/]");
        Thread.Sleep(2000);
    }

    public static void Borrar(ToDo todo){
        string name = AnsiConsole.Ask<string>("[bold]Nombre: [/]");
        todo.Remove(name);
    }

    public static void Guardar(ToDo todo){
        todo.Save(PATH);
    }

    public static void PrintList(ToDo todo){
        Table table = new Table().AddColumn("Nombre").AddColumn("Fecha y hora");
        foreach(string[] l in todo.GetTareasStrings())
            table.AddRow(l);
        table.Title = new TableTitle("Tareas");
        table.RoundedBorder();
        AnsiConsole.Write(table);
    }
}

public class ToDo{
    private List<Tarea> tareas;
    public ToDo(){
        tareas = new List<Tarea>();
    }

    public void Add(Tarea tarea){
        tareas.Add(tarea);
    }

    public void Remove(string name){
        tareas.RemoveAll(t => t.Nombre == name);
    }

    public Tarea[] GetTareas(){
        return tareas.ToArray();
    }

    public string[][] GetTareasStrings(){
        string[][] ret = new string[tareas.Count][];
        for(int i = 0; i < tareas.Count; i++){
            ret[i] = new string[]
                {
                    tareas[i].Nombre, 
                    tareas[i].time.ToString()
                };
        }
        return ret;
    }

    public void Load(FileStream file){
        tareas = JsonSerializer.Deserialize<List<Tarea>>(file) ?? tareas;
        file.Close();
    }

    public void Save(string path){
        string json = JsonSerializer.Serialize(tareas);
        File.WriteAllText(path, json);
    }
}

[Serializable]
public class Tarea{
    public string Nombre {get; set;}
    public DateTime time {get; set;}

    public Tarea(){
        Nombre = "";
    }
    public Tarea(string n, DateTime t){
        Nombre = n;
        time = t;
    }
}
