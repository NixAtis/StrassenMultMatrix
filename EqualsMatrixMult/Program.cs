using System;
using System.Diagnostics;

namespace Matrix
{
    class StrassenMatrixMultiplication
    {
        public static int[,] Multiply(int[,] a, int[,] b, int k)
        {
            int n = a.GetLength(0);

            if (n <= k)
            {
                // Если матрицы маленькие, используем обычное умножение матриц
                return StandardMatrixMultiply(a, b);
            }

            // Разделяем матрицы на подматрицы
            int[,] a11, a12, a21, a22;
            int[,] b11, b12, b21, b22;
            SplitMatrix(a, out a11, out a12, out a21, out a22);
            SplitMatrix(b, out b11, out b12, out b21, out b22);

            // Вычисляем 7 промежуточных матриц
            int[,] p1 = Multiply(a11, Subtract(b12, b22),k);
            int[,] p2 = Multiply(Add(a11, a12), b22,k);
            int[,] p3 = Multiply(Add(a21, a22), b11,k);
            int[,] p4 = Multiply(a22, Subtract(b21, b11),k);
            int[,] p5 = Multiply(Add(a11, a22), Add(b11, b22),k);
            int[,] p6 = Multiply(Subtract(a12, a22), Add(b21, b22),k);
            int[,] p7 = Multiply(Subtract(a11, a21), Add(b11, b12),k);

            // Собираем результат из 7 промежуточных матриц
            int[,] c11 = Add(Subtract(Add(p5, p4), p2), p6);
            int[,] c12 = Add(p1, p2);
            int[,] c21 = Add(p3, p4);
            int[,] c22 = Subtract(Subtract(Add(p5, p1), p3), p7);

            // Объединяем 4 подматрицы, чтобы получить конечный результат
            int[,] result = new int[n, n];
            CopySubMatrix(c11, result, 0, 0);
            CopySubMatrix(c12, result, 0, n / 2);
            CopySubMatrix(c21, result, n / 2, 0);
            CopySubMatrix(c22, result, n / 2, n / 2);

            return result;
        }

        public static int[,] StandardMatrixMultiply(int[,] a, int[,] b)
        {
            int n = a.GetLength(0);
            int[,] result = new int[n, n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    for (int k = 0; k < n; k++)
                    {
                        result[i, j] += a[i, k] * b[k, j];
                    }
                }
            }

            return result;
        }

        private static void SplitMatrix(int[,] original, out int[,] part1, out int[,] part2, out int[,] part3, out int[,] part4)
        {
            int n = original.GetLength(0);
            int half = n / 2;

            part1 = new int[half, half];
            part2 = new int[half, half];
            part3 = new int[half, half];
            part4 = new int[half, half];

            for (int i = 0; i < half; i++)
            {
                for (int j = 0; j < half; j++)
                {
                    part1[i, j] = original[i, j];
                    part2[i, j] = original[i, j + half];
                    part3[i, j] = original[i + half, j];
                    part4[i, j] = original[i + half, j + half];
                }
            }
        }

        private static void CopySubMatrix(int[,] source, int[,] destination, int startRow, int startCol)
        {
            int n = source.GetLength(0);

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    destination[startRow + i, startCol + j] = source[i, j];
                }
            }
        }

        private static int[,] Add(int[,] a, int[,] b)
        {
            int n = a.GetLength(0);
            int[,] result = new int[n, n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    result[i, j] = a[i, j] + b[i, j];
                }
            }

            return result;
        }

        private static int[,] Subtract(int[,] a, int[,] b)
        {
            int n = a.GetLength(0);
            int[,] result = new int[n, n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    result[i, j] = a[i, j] - b[i, j];
                }
            }

            return result;
        }
    }

    class Program
    {
		static int[,] GenerateRandomArray(int rows, int cols)
		{
			Random random = new Random();
			int[,] array = new int[rows, cols];

			for (int i = 0; i < rows; i++)
			{
				for (int j = 0; j < cols; j++)
				{
					array[i, j] = random.Next(1, 5); // генерируем случайное число от 1 до 100
				}
			}

			return array;
		}
		static void PrintArray(int[,] array)
        {
			for (int i = 0; i < array.GetLength(0); i++)
			{
				for (int j = 0; j < array.GetLength(1); j++)
					Console.Write(array[i, j] + " ");
				Console.WriteLine();
			}
		}

        static int[,] ToPower2(int[,] a)
        {
            int n = a.GetLength(0);
            int pow1 = (int)Math.Floor(Math.Log(n,2));
            if (n > 0 && (n & (n - 1)) != 0) pow1++;
            n = (int)Math.Pow(2, pow1);
            int[,] new_a = new int[n, n];
            for (int i = 0; i < a.GetLength(0); i++)
            {
                for (int j = 0; j < a.GetLength(0); j++)
                {
                    new_a[i, j] = a[i, j];
                }
            }
            return new_a;
        }
        static void Main(string[] args)
		{
            int[] count_arr = { 10, 100 }; //коллекция размерности матриц (n)
            var timer1 = new Stopwatch();
			var timer2 = new Stopwatch();

			Console.WriteLine("1. Генерация тестовых значений\n" +
							  "2. Ручной ввод");
			int selectMenu = int.Parse(Console.ReadLine());
			while (true)
			{
				if (selectMenu == 1)
				{
                    for (int i = 0; i < count_arr.Length; i++) 
                    {
                        int rows, cols, rows2, cols2;
                        rows = cols = rows2 = cols2 = count_arr[i];

                        var arr1 = GenerateRandomArray(rows, cols);
                        var arr2 = GenerateRandomArray(rows2, cols2);

                        Console.WriteLine($"i =  {0} ({count_arr[i]})");
                        #region тревиальный

                        timer1.Start();
                        var resC = StrassenMatrixMultiplication.StandardMatrixMultiply(arr1, arr2);
                        timer1.Stop();
                        #endregion

                        #region Штрассен
                        timer2.Start();
                        int[,] res = StrassenMatrixMultiplication.Multiply(ToPower2(arr1), ToPower2(arr2), 64);
                        timer2.Stop();
                        #endregion



                        Console.WriteLine($"(Штрассен) Время выполнения: {timer2.ElapsedMilliseconds / 1e3}с");
                        Console.WriteLine($"(Тривиальный) Время выполнения: {timer1.ElapsedMilliseconds / 1e3}с");
                        timer2.Reset(); timer1.Reset();
                        Console.WriteLine();
                    }
                    

                    Console.Read();
                    return;
				}
				if (selectMenu == 2)
                {
                    Console.WriteLine("Первая матрица");
					Console.WriteLine("Введите количество строк:");
					int rows = int.Parse(Console.ReadLine());
					Console.WriteLine("Введите количество столбцов:");
					int cols = int.Parse(Console.ReadLine());
					int[,] arr1 = new int[rows, cols];

                    for (int i = 0; i < rows; i++)
                    {
                        for (int j = 0; j < cols; j++)
                        {
							Console.Write($"a[{i},{j}] =  ");
							arr1[i, j] = int.Parse(Console.ReadLine());
                        }
                    }
					Console.Clear();


					Console.WriteLine("Вторая матрица массив");
					int rows2 = cols;
					Console.WriteLine("Введите количество столбцов:");
					int cols2 = int.Parse(Console.ReadLine());
					int[,] arr2 = new int[rows, cols];

					for (int i = 0; i < rows2; i++)
					{
						for (int j = 0; j < cols2; j++)
						{
							Console.Write($"a[{i},{j}] =  ");
							arr2[i, j] = int.Parse(Console.ReadLine());
						}
					}
					Console.Clear();

					Console.WriteLine("Массив 1:"); PrintArray(arr1); Console.WriteLine();
					Console.WriteLine("Массив 2:"); PrintArray(arr2); Console.WriteLine();


                    #region тривиальный
                    timer1.Start();
                    var resC = StrassenMatrixMultiplication.StandardMatrixMultiply(arr1, arr2);
                    timer1.Stop();
                    #endregion

                    #region Штрассен
                    timer2.Start();
                    int[,] res = StrassenMatrixMultiplication.Multiply(ToPower2(arr1), ToPower2(arr2), 64);
                    timer2.Stop();
                    #endregion


                    Console.WriteLine($"(Тривиальный) Время выполнения: {timer1.ElapsedMilliseconds}мс");
					Console.WriteLine($"(Штрассен) Время выполнения: {timer2.ElapsedMilliseconds}мс");
					Console.Read();
					return;
				}
			}


		}
    }
}
