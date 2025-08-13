// See https://aka.ms/new-console-template for more information

using kigyoCli;

var kigyo = new Kigyo();

kigyo.Start();

while (true)
{
    var key = Console.ReadKey(true);
    kigyo.Move(key.Key);
}