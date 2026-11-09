
namespace PracticaFiguras
{
    class Triangulo : Figura
    {
        private double lado1;
        private double lado2;
        private double lado3;
        private double altura;

        public Triangulo(double lado1, double lado2,
                         double lado3, double altura)
        {
            this.lado1 = lado1;
            this.lado2 = lado2;
            this.lado3 = lado3;
            this.altura = altura;
        }

        public override double CalcularArea()
        {
            return (lado1 * altura) / 2;
        }

        public override double CalcularPerimetro()
        {
            return lado1 + lado2 + lado3;
        }
    }
}
