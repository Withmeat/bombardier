using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bombardier_wpf.Persistence
{
    public class bombardDataAccess : IbombardDataAccess
    {
        public async Task<gameMap> LoadAsync(String path)
        {
            try
            {
                string filePath = System.IO.Path.GetFullPath(path);

                using (StreamReader reader = new StreamReader(filePath))
                {
                    int[] firstLine = Array.ConvertAll(reader.ReadLine().Split(' '), int.Parse);
                    int tableSize = firstLine[0];

                    gameMap table = new gameMap(tableSize);
                    for (int i = 0; i < tableSize; i++)
                    {
                        String line = await reader.ReadLineAsync();

                        for (int j = 0; j < line.Length; j++)
                        {
                            double k = Char.GetNumericValue(line[j]);
                            table[i, j].Add((Player)k);
                        }
                    }
                    table.InitDirections();
                    return table;

                }
            }
            catch
            {

                throw new bombardDataException();
            }
        }


        public async Task SaveAsync(String path, gameMap table)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(path)) // fájl megnyitása
                {
                    writer.Write(table.Size); // kiírjuk a méreteket
                    await writer.WriteLineAsync();
                    for (Int32 i = 0; i < table.Size; i++)
                    {
                        for (Int32 j = 0; j < table.Size; j++)
                        {

                            if (table[i, j].Contains(Player.Ellenség))
                                await writer.WriteAsync(((int)Player.Ellenség).ToString());
                            else if (table[i, j].Contains(Player.Bombázó))
                                await writer.WriteAsync(((int)Player.Bombázó).ToString());
                            else if (table[i, j].Contains(Player.Fal))
                                await writer.WriteAsync(((int)Player.Fal).ToString());
                            else if (table[i, j].Contains(Player.Talaj))
                                await writer.WriteAsync(((int)Player.Talaj).ToString());

                        }
                        await writer.WriteLineAsync();
                    }
                }

            }
            catch
            {

                throw new bombardDataException();
            }
        }
    }
}
