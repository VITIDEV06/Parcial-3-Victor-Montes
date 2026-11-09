using Newtonsoft.Json;

var persona = new
{
    Nombre = "Cristell",
    Edad = 20,
    Ciudad = "Ingenieria en Sistemas"
};

string json = JsonConvert.SerializeObject(persona);

Console.WriteLine(json);