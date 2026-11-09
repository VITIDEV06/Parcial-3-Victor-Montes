using System;
using System;
using PracticaFiguras;

class Program
{
    static void Main(string[] args)
    {
        // Arreglo unidimensional de tipo Figura
        Figura[] figuras = new Figura[]
        {
            new Rectangulo(10, 5),
            new Cuadrado(4),
            new Triangulo(6, 5, 5, 4)
        };

        Console.WriteLine("=================================");
        Console.WriteLine("       CALCULO DE FIGURAS");
        Console.WriteLine("=================================");

        // Recorrido polimorfico
        foreach (Figura figura in figuras)
        {
            Console.WriteLine("\nFigura: " + figura.GetType().Name);

            Console.WriteLine("Area: " + figura.CalcularArea());

            Console.WriteLine("Perimetro: " + figura.CalcularPerimetro());

            Console.WriteLine("---------------------------------");
        }

        Console.ReadKey();
    }
}
