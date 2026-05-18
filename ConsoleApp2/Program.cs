using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Text.Json;
using System.Linq;

class Jogador
{
    public string Nome { get; set; }
    public int Pontos { get; set; }
    public int Dinheiro { get; set; }
    public List<string> Itens { get; set; }
    public Dictionary<string, int> Minerios { get; set; }
}

class Program
{
    static void Main()
    {
        string shopText = "\n" +
            "- Espada - 1000 coins\n" +
            "- Picareta - 1000 coins\n" +
            "\n" +
            "Digite comprar (item)";

        string FileName = "save.json";

        if (!File.Exists(FileName))
        {
            Console.WriteLine("Seja bem vindo");
            Console.Write("digite seu nome: ");
            string nome = Console.ReadLine();
            
            
            Jogador jogador = new Jogador
            {
                Nome = nome,
                Pontos = 100,
                Dinheiro = 1000,
                Itens = new List<string>(),
                Minerios = new Dictionary<string, int>()
            };
            string json = JsonSerializer.Serialize(jogador, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(FileName, json);
            Console.WriteLine($"Olá {nome}");
        }
        else
        {
            Jogador jogador = JsonSerializer.Deserialize<Jogador>(File.ReadAllText(FileName));
            Console.WriteLine("Bem vindo de novo " + jogador.Nome);
            
        }
        Jogador p = JsonSerializer.Deserialize<Jogador>(File.ReadAllText(FileName));

        while (true)
        {
            string input = Console.ReadLine();
            string[] partes = input.Split(' ');

            string comando = partes[0];
            string argumento = partes.Length > 1 ? partes[1] : "";

            switch (comando)
            {
                // adm
                case "h":
                    Console.WriteLine("Foi dado muito dinheiro pra vc");
                    p.Dinheiro += 1000000;
                    Salvar(FileName, p);
                    break;
                // comandos

                case "point":
                    Console.WriteLine($"Você tem {p.Pontos} Pontos");
                    break;

                case "dinheiro":
                    Console.WriteLine($"Você tem {p.Dinheiro} Moedas");
                    break;
                
                case "limpar":
                    Console.Clear();
                    break;
                case "reset":
                    if (File.Exists(FileName))
                    {
                        File.Delete(FileName);
                        Console.WriteLine("Save apagado.");
                        Console.Write("Digite seu novo nome: ");
                        string novoNome = Console.ReadLine();

                        p = new Jogador
                        {
                            Nome = novoNome,
                            Pontos = 100,
                            Dinheiro = 1000,
                            Itens = new List<string>(),
                            Minerios = new Dictionary<string, int>()
                        };

                        Salvar(FileName, p);
                        Console.WriteLine("Novo save criado!");
                    }

                    break;
                case "itens":
                    Console.WriteLine("Seu inventário:");
                    if (p.Itens.Count == 0)
                    {
                        Console.WriteLine("- vazio");
                        Console.Beep();
                    }
                    else
                    {
                        foreach (string item in p.Itens)
                        {
                            Console.WriteLine("- " + item);
                        }
                    }
                    break;
                case "minerios":
                    Console.WriteLine("Seu inventário:");
                    if (p.Minerios.Count() == 0)
                    {
                        Console.WriteLine("- sem minerios");
                        Console.Beep();
                    }
                    else
                    {
                        foreach (var item in p.Minerios)
                        {
                            Console.WriteLine("- " + item.Key + " x" + item.Value);
                        }
                    }
                    break;
                case "loja":
                    Console.WriteLine(shopText);
                    break;

                case "comprar":
                    switch (argumento)
                    {
                        case "espada":
                            Comprar(FileName, p, "espada", 1000);
                            break;
                        case "picareta":
                            Comprar(FileName, p, "picareta", 1000);
                            break;
                    }

                    break;
                case "vender":
                    if (partes.Length < 3)
{
                        Console.WriteLine("Use: vender ferro 3");
                        break;
                    }

                    string minerio = partes[1];
                    int quantidade = int.Parse(partes[2]);

                    if (!p.Minerios.ContainsKey(minerio))
                    {
                        Console.WriteLine("Você não possui esse minério.");
                        break;
                    }

                    if (p.Minerios[minerio] < quantidade)
                    {
                        Console.WriteLine("Quantidade insuficiente.");
                        break;
                    }

                    int valor = 0;

                    switch (minerio.ToLower())
                    {
                        case "carvão": valor = 10; break;
                        case "ferro": valor = 25; break; 
                        case "ouro": valor = 50; break; 
                        case "diamante": valor = 100; break;

                        default:
                            Console.WriteLine("Minério inválido.");
                            break;
                    }

                    int total = valor * quantidade;
                    p.Minerios[minerio] -= quantidade;

                    if (p.Minerios[minerio] <= 0)
                    {
                        p.Minerios.Remove(minerio);
                    }
                    p.Dinheiro += total;

                    Console.WriteLine($"Você vendeu {minerio} x{quantidade}");
                    Console.WriteLine($"Recebeu {total} moedas");
                    Salvar(FileName, p);
                    break;
                case "vendertudo":

                    if (p.Minerios.Count == 0)
                    {
                        Console.WriteLine("Você não possui minérios.");
                        break;
                    }

                    int totalGanho = 0;

                    foreach (var minerio in p.Minerios.ToList())
                    {
                        int valor = 0;

                        switch (minerio.Key.ToLower())
                        {
                            case "carvão":
                                valor = 10;
                                break;

                            case "ferro":
                                valor = 25;
                                break;

                            case "ouro":
                                valor = 50;
                                break;

                            case "diamante":
                                valor = 100;
                                break;
                        }

                        int ganho = valor * minerio.Value;

                        totalGanho += ganho;

                        Console.WriteLine($"Vendido {minerio.Key} x{minerio.Value} por {ganho}");
                    }

                    p.Minerios.Clear();

                    p.Dinheiro += totalGanho;

                    Console.WriteLine($"Total recebido: {totalGanho}");

                    Salvar(FileName, p);

                    break;
                case "minerar":
                    if (p.Itens.Contains("picareta"))
                    {
                        addMinerio(FileName, p);
                    }
                    else
                    {
                        Console.WriteLine("Você não possui picareta");
                        Console.Beep();
                    }
                    break;
                case "sair":
                    return;

                default:
                    Console.WriteLine("Comando inválido.");
                    break;
            }
        }

        static void Salvar(string fileName, Jogador p)
        {
            string json = JsonSerializer.Serialize(p, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(fileName, json);
        }

        static Jogador Carregar(string fileName)
        {
            string json = File.ReadAllText(fileName);
            return JsonSerializer.Deserialize<Jogador>(json);
        }

        Console.ReadKey();

        static void Comprar(string FileName, Jogador p, string Item, int Valor, int Quantidade = 1)
        {
            if (p.Dinheiro >= Valor)
            {
                p.Dinheiro -= Valor;
                p.Itens.Add(Item);

                Console.WriteLine($"Você comprou {Item} x{Quantidade}");
                Salvar(FileName, p);
            }
            else
            {
                Console.WriteLine("Você não tem dinheiro suficiente.");
            }
        }

        static void addMinerio(string FileName, Jogador p)
        {
            Random random = new Random();
            int numero = random.Next(1, 6);
            int chance = random.Next(1, 101);
            string minerio;

            if (chance <= 50) { minerio = "Carvão"; }
            else if (chance <= 80) { minerio = "Ferro"; }
            else if (chance <= 95) { minerio = "Ouro"; }
            else { minerio = "Diamante"; }

            Console.WriteLine($"Você minerou: {minerio} x{numero}");

            if (p.Minerios.ContainsKey(minerio))
            {
                p.Minerios[minerio] += numero;
            }
            else
            {
                p.Minerios[minerio] = numero;
            }
            Salvar(FileName, p);
        }
    }
}