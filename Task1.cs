using System;
using System.Collections.Generic;
using System.Text;

namespace StoreSimulation
{
    struct Product
    {
        public string Name;
        public int Price;
        public int Quantity;

        public Product(string name, int price, int quantity)
        {
            Name = name;
            Price = price;
            Quantity = quantity;
        }
    }

    class Customer
    {
        public int CardNumber { get; }
        public int Money { get; private set; }
        public int ItemsBought { get; private set; }
        public int MoneySpent { get; private set; }

        public Customer(Random rnd)
        {
            CardNumber = rnd.Next(1, 1001);
            Money = rnd.Next(1000, 10001);
            ItemsBought = 0;
            MoneySpent = 0;
        }

        public bool CanAfford(int price)
        {
            return Money >= price;
        }

        public void Buy(int price)
        {
            Money -= price;
            MoneySpent += price;
            ItemsBought++;
        }
    }

    class Store
    {
        public List<Product> Products { get; }
        public int Profit { get; private set; }

        public Store()
        {
            Products = new List<Product>();
            Profit = 0;
        }

        public void AddProduct(string name, int price, int quantity)
        {
            Products.Add(new Product(name, price, quantity));
        }

        public bool HasAvailableProducts()
        {
            for (int i = 0; i < Products.Count; i++)
            {
                if (Products[i].Quantity > 0)
                    return true;
            }
            return false;
        }

        public int GetMinAvailablePrice()
        {
            int minPrice = int.MaxValue;
            for (int i = 0; i < Products.Count; i++)
            {
                if (Products[i].Quantity > 0 && Products[i].Price < minPrice)
                    minPrice = Products[i].Price;
            }
            return minPrice;
        }

        public bool CanAnyoneBuy(List<Customer> customers)
        {
            if (!HasAvailableProducts())
                return false;

            int minPrice = GetMinAvailablePrice();
            for (int i = 0; i < customers.Count; i++)
            {
                if (customers[i].CanAfford(minPrice))
                    return true;
            }
            return false;
        }

        public void TryPurchase(Customer customer, int productIndex)
        {
            Product item = Products[productIndex];
            if (item.Quantity > 0 && customer.CanAfford(item.Price))
            {
                item.Quantity--;
                Products[productIndex] = item;

                customer.Buy(item.Price);
                Profit += item.Price;

                Console.WriteLine($"Покупець #{customer.CardNumber} купив \"{item.Name}\" за {item.Price} грн (залишок: {customer.Money} грн)");
            }
        }

        public void RunSimulation(List<Customer> customers, Random rnd)
        {
            while (CanAnyoneBuy(customers))
            {
                int customerIndex = rnd.Next(customers.Count);
                int productIndex = rnd.Next(Products.Count);
                TryPurchase(customers[customerIndex], productIndex);
            }
        }

        public void PrintResults(List<Customer> customers)
        {
            Customer maxItemsCustomer = customers[0];
            Customer maxSpentCustomer = customers[0];

            for (int i = 1; i < customers.Count; i++)
            {
                if (customers[i].ItemsBought > maxItemsCustomer.ItemsBought)
                    maxItemsCustomer = customers[i];

                if (customers[i].MoneySpent > maxSpentCustomer.MoneySpent)
                    maxSpentCustomer = customers[i];
            }

            Console.WriteLine("-------------------");
            Console.WriteLine($"Загальний прибуток магазину: {Profit} грн");
            Console.WriteLine($"Найбільше товарів купив покупець з карткою #{maxItemsCustomer.CardNumber} ({maxItemsCustomer.ItemsBought} шт.)");
            Console.WriteLine($"Найбільше грошей витратив покупець з карткою #{maxSpentCustomer.CardNumber} ({maxSpentCustomer.MoneySpent} грн)");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Random rnd = new Random();

            Store store = new Store();
            store.AddProduct("Клавіатура", 1500, 5);
            store.AddProduct("Мишка", 800, 8);
            store.AddProduct("Навушники", 2200, 4);
            store.AddProduct("Килимок", 400, 10);
            store.AddProduct("Флешка 64GB", 350, 12);
            store.AddProduct("Вебкамера", 1900, 3);

            Console.Write("Введіть кількість покупців: ");
            int count = int.Parse(Console.ReadLine());

            List<Customer> customers = new List<Customer>();
            for (int i = 0; i < count; i++)
            {
                Customer c = new Customer(rnd);
                customers.Add(c);
                Console.WriteLine($"Створено покупця: картка #{c.CardNumber}, баланс: {c.Money} грн");
            }

            Console.WriteLine("-------------------");
            store.RunSimulation(customers, rnd);
            store.PrintResults(customers);
        }
    }
}