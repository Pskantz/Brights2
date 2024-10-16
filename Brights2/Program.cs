using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Program
{
    static void Main()
    {
        // Load allowed fruits from file
        var allowedFruits = LoadAllowedFruits("allowed-fruits.txt");

        if (allowedFruits.Count == 0)
        {
            Console.WriteLine("No allowed fruits found. Exiting program.");
            return;
        }

        // Initialize the dictionary to store fruits and their prices
        var fruits = new Dictionary<string, decimal>();

        // Load previously saved data, if any
        if (File.Exists("fruits.csv"))
        {
            fruits = LoadFruitData("fruits.csv");
            Console.WriteLine("Previous fruit data loaded successfully.");
        }

        // Loop to get fruits and prices
        while (true)
        {
            Console.Write("Enter a fruit (or 'done' to finish): ");
            string? fruit = Console.ReadLine()?.Trim() ?? "";

            // Handle special command to finish
            if (fruit.ToLower() == "done")
            {
                break;
            }

            // Validate fruit name
            if (!allowedFruits.Contains(fruit.ToLower()))
            {
                Console.WriteLine("This fruit is not allowed. Please enter a valid fruit from the allowed list.");
                continue;
            }

            // Check if the fruit is already in the dictionary
            if (fruits.ContainsKey(fruit))
            {
                Console.WriteLine("That fruit is already in the dictionary. Please enter a different fruit.");
                continue;
            }

            // Get and validate the price input
            decimal price;
            while (true)
            {
                Console.Write($"Enter the price for {fruit}: ");
                string? priceInput = Console.ReadLine() ?? "";

                if (decimal.TryParse(priceInput, out price) && price >= 0)
                {
                    break; // Valid price, exit loop
                }
                else
                {
                    Console.WriteLine("Invalid price. Please enter a valid decimal number greater than or equal to 0.");
                }
            }

            // Add the fruit and price to the dictionary
            fruits[fruit] = price;

            // Immediately save the fruit and price to the CSV file
            SaveFruitData("fruits.csv", fruit, price);

            Console.WriteLine($"Fruit '{fruit}' with price {price:C2} saved.");
        }

        // Check if any fruits were entered
        if (fruits.Count == 0)
        {
            Console.WriteLine("No fruits were entered.");
        }
        else
        {
            // Sort the fruits by price and display them
            Console.WriteLine("\nFruits and their prices, sorted by price (cheapest first):");
            foreach (var fruit in fruits.OrderBy(f => f.Value))
            {
                Console.WriteLine($"{fruit.Key}: ${fruit.Value:F2}");
            }
        }
    }

    // Load allowed fruits from a file
    static HashSet<string> LoadAllowedFruits(string filePath)
    {
        var allowedFruits = new HashSet<string>();

        if (File.Exists(filePath))
        {
            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                var fruit = line.Trim().ToLower();
                if (!string.IsNullOrEmpty(fruit))
                {
                    allowedFruits.Add(fruit);
                }
            }
        }
        else
        {
            Console.WriteLine($"File '{filePath}' not found. Please make sure it exists.");
        }

        return allowedFruits;
    }

    // Save fruit and price to a CSV file (appending enabled)
    static void SaveFruitData(string filePath, string fruit, decimal price)
    {
        using (var writer = new StreamWriter(filePath, append: true)) // Enable appending
        {
            writer.WriteLine($"{fruit},{price}");
        }
    }

    // Load fruit data from a CSV file
    static Dictionary<string, decimal> LoadFruitData(string filePath)
    {
        var fruits = new Dictionary<string, decimal>();

        if (File.Exists(filePath))
        {
            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                var parts = line.Split(',');
                if (parts.Length == 2 && decimal.TryParse(parts[1], out decimal price))
                {
                    fruits[parts[0]] = price;
                }
            }
        }

        return fruits;
    }
}
