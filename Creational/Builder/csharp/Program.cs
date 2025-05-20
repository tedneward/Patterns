using System.Collections.Generic;

var director = new Basic.Director ();
var product = director.Construct ();
Console.WriteLine(product);

var fluentbuilder = new Fluent.Builder ();
product = fluentbuilder.Begin()
    .Engine
    .SteeringWheel
    .Tire()
    .Tire()
    .Build();
Console.WriteLine(product);

var fnbuilder = new Functional.Builder();
product = fnbuilder.Begin()
    .Engine
    .SteeringWheel
    .Tire()
    .Tire()
    .Build();
Console.WriteLine(product);

var cfnbuilder = new Functional.ComposedBuilder();
product = cfnbuilder.Begin()
    .Engine
    .SteeringWheel
    .Tire(4)
    .Build();
Console.WriteLine(product);

// ---------

class Product
{
    public List<String> Parts = new List<String>();
    public override String ToString()
    {
        return "Product has " + Parts.Aggregate("", (total, next) => total + next + " ");
    }
}

// ---------

namespace Basic
{
    abstract class Builder
    {
        public abstract void BuildPart ();
        public abstract Product Construct ();
    }

    class ConcreteBuilder : Builder
    {
        private Product product = new Product();
        private int part = 0;

        public override void BuildPart()
        {            
            product.Parts.Add ("Part #" + (part++));
        }

        public override Product Construct()
        {
            return product;
        }
    }

    class Director
    {
        private Builder builder = new ConcreteBuilder();
        public Product Construct()
        {
            for (int i = 0; i < 5; i++)
                builder.BuildPart ();

            return builder.Construct ();
        }
    }
}
namespace Fluent
{
    class Builder
    {
        private Product product;
        public Builder Begin() 
        {
            product = new Product();
            return this;
        }
        public Builder Engine 
        {
            get 
            {
                product.Parts.Add ("Engine");
                return this;
            }
        }
        public Builder SteeringWheel
        {
            get 
            {
                product.Parts.Add("Steering Wheel");
                return this;
            }
        }
        public Builder Tire() 
        {
            product.Parts.Add("Tire");
            return this;
        }
        public Product Build() 
        {
            return product;
        }
    }

    class Guarded
    {
        private Product product;
        public Guarded Begin() {
            product = new Product();
            return this;
        }
        public Guarded Engine {
            get 
            {
                product.Parts.Add ("Engine");
                return this;
            }
        }
        public Guarded SteeringWheel
        {
            get 
            {
                product.Parts.Add("Steering Wheel");
                return this;
            }
        }
        public Guarded Tire() 
        {
            product.Parts.Add("Tire");
            return this;
        }
        public Product Build() 
        {
            if (product.Parts.Count < 4)
                throw new Exception ("Product doesn't have enough parts to it yet");

            if (product.Parts.Contains ("Engine") == false)
                throw new Exception ("Product must have an Engine");

            if (product.Parts.Find((str) => str == "Tire").Length < 2)
                throw new Exception ("Product must have at least 2 Tires");

            return product;
        }
    }
}

namespace Functional
{
    class Builder
    {
        private List<Func<Product, Product>> steps = 
            new List<Func<Product, Product>>();

        public Builder Begin() {
            steps.Clear ();
            return this;
        }
        public Builder Engine {
            get 
            {
                steps.Add ((product) => {
                    product.Parts.Add("Engine");
                    return product;
                });
                return this;
            }
        }
        public Builder SteeringWheel
        {
            get 
            {
                steps.Add ((product) => {
                    product.Parts.Add("Steering Wheel");
                    return product;
                });
                return this;
            }
        }
        public Builder Tire() {
            steps.Add ((product) => {
                product.Parts.Add("Tire");
                return product;
            });
            return this;
        }
        public Product Build() {
            var working = new Product ();
            foreach (var step in steps) {
                working = step (working);
            }
            return working;
        }
    }

    class ComposedBuilder
    {
        private static Func<A,C> Compose<A,B,C>(Func<A,B> f1, Func<B, C> f2) 
        {
            return (a) => f2(f1(a));
        }

        private Func<Product, Product> fn = null;

        public ComposedBuilder Begin() {
            fn = (ignored) => new Product ();
            return this;
        }
        public ComposedBuilder Engine {
            get 
            {
                fn = Compose (fn, (product) => {
                    product.Parts.Add("Engine");
                    return product;
                });
                return this;
            }
        }
        public ComposedBuilder SteeringWheel
        {
            get 
            {
                fn = Compose (fn, (product) => {
                    product.Parts.Add("Steering Wheel");
                    return product;
                });
                return this;
            }
        }
        public ComposedBuilder Tire() {
            fn = Compose (fn, (product) => {
                product.Parts.Add("Tire");
                return product;
            });
            return this;
        }
        public ComposedBuilder Tire(int numberOfTires) {
            fn = Compose(fn, (product) => {
                for (int i=0; i<numberOfTires; i++)
                    product.Parts.Add("Tire");
                return product;
            });
            return this;
        }
        public Product Build() {
            return fn(null);
        }
    }
}


