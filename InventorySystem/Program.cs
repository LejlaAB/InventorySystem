
using System;
using System.Collections.Generic;
using System.Linq;

namespace InventorySystem
{


    abstract class Item
    {
        public string Name { get; }
        public decimal PricePerUnit { get; }

        protected Item(string name, decimal price)
        {
            Name = name;
            PricePerUnit = price;
        }
    }

    class BulkItem : Item
    {
        public string Unit { get; }

        public BulkItem(string name, decimal price, string unit) : base(name, price)
        {
            Unit = unit;
        }
    }

    class UnitItem : Item
    {
        public double Weight { get; }

        public UnitItem(string name, decimal price, double weight) : base(name, price)
        {
            Weight = weight;
        }
    }

    class Customer
    {
        public string Name { get; }

        public Customer(string name)
        {
            Name = name;
        }
    }

    class OrderLine
    {
        public Item Item { get; }
        public double Quantity { get; }

        public decimal LinePrice => (decimal)Quantity * Item.PricePerUnit;

        public OrderLine(Item item, double quantity)
        {
            Item = item;
            Quantity = quantity;
        }
    }

    class Order
    {
        public Customer Customer { get; }
        public List<OrderLine> Lines { get; } = new();
        public DateTime? ProcessedAt { get; private set; }

        public Order(Customer customer)
        {
            Customer = customer;
        }

        public void AddItem(Item item, double quantity)
        {
            Lines.Add(new OrderLine(item, quantity));
        }

        public decimal TotalPrice => Lines.Sum(l => l.LinePrice);

        public void MarkProcessed()
        {
            ProcessedAt = DateTime.Now;
        }
    }

    class Inventory
    {
        private readonly Dictionary<string, Item> items = new();

        public void AddItem(Item item)
        {
            items[item.Name] = item;
        }

        public Item GetItem(string name)
        {
            return items[name];
        }

        public void ShowAll()
        {
            Console.WriteLine("\n--- Lager ---");
            foreach (var item in items.Values)
                Console.WriteLine($"{item.Name} - {item.PricePerUnit} kr. pr. enhed");
        }
    }

    class OrderBook
    {
        private readonly Queue<Order> queuedOrders = new();
        private readonly List<Order> processedOrders = new();
        public decimal TotalRevenue { get; private set; }

        public void Enqueue(Order order)
        {
            queuedOrders.Enqueue(order);
        }

        public void ProcessNext()
        {
            if (queuedOrders.Count == 0)
            {
                Console.WriteLine("\nIngen ordrer i køen.");
                return;
            }

            var next = queuedOrders.Dequeue();
            next.MarkProcessed();
            processedOrders.Add(next);
            TotalRevenue += next.TotalPrice;

            Console.WriteLine($"\n Ordre fra {next.Customer.Name} er behandlet ({next.TotalPrice} kr.)");
        }

        public void ShowStatus()
        {
            Console.WriteLine("\n--- Kø (venter) ---");
            foreach (var o in queuedOrders)
                Console.WriteLine($"- {o.Customer.Name}, {o.Lines.Count} linjer, {o.TotalPrice} kr.");

            Console.WriteLine("\n--- Færdigbehandlede ---");
            foreach (var o in processedOrders)
                Console.WriteLine($"- {o.Customer.Name}, {o.Lines.Count} linjer, {o.TotalPrice} kr., {o.ProcessedAt}");

            Console.WriteLine($"\n Total omsætning: {TotalRevenue} kr.");
        }
    }

    class Program
    {
        static void Main()
        {
            Console.WriteLine("Velkommen til Inventory System!\n");

            var inv = new Inventory();
            inv.AddItem(new BulkItem("Hydraulic oil", 5.0m, "L"));
            inv.AddItem(new UnitItem("PLC module", 1250m, 1.0));
            inv.AddItem(new UnitItem("Servo motor", 2300m, 2.0));

            var sara = new Customer("Sara");
            var carl = new Customer("Carl");

            var order1 = new Order(sara);
            order1.AddItem(inv.GetItem("Hydraulic oil"), 15);
            order1.AddItem(inv.GetItem("PLC module"), 2);
            order1.AddItem(inv.GetItem("Servo motor"), 1);

            var order2 = new Order(carl);
            order2.AddItem(inv.GetItem("Hydraulic oil"), 10);

            var book = new OrderBook();
            book.Enqueue(order1);
            book.Enqueue(order2);

            bool running = true;
            while (running)
            {
                Console.WriteLine("\nVælg handling:");
                Console.WriteLine("1. Vis lager");
                Console.WriteLine("2. Vis ordrer");
                Console.WriteLine("3. Behandl næste ordre");
                Console.WriteLine("4. Afslut");
                Console.Write("Valg: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        inv.ShowAll();
                        break;
                    case "2":
                        book.ShowStatus();
                        break;
                    case "3":
                        book.ProcessNext();
                        break;
                    case "4":
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Ugyldigt valg.");
                        break;
                }
            }

            Console.WriteLine("\nProgram afsluttet.");
        }
    }
}


