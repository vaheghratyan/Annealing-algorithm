using System;

namespace MainProgram {
    public class Program {
        const int N = 50; // размер доски и число ферзей
        const double Tn = 30.0; // начальная температура
        const double Tk = 0.1; // конечная температура
        const double Alfa = 0.98; // скорость охлаждения
        const int IterationsNumber = 80; // число итераций при смене T
        int[,] RoadWeights = new int[N, N];

        public class TMember {
            public int[] Plan = new int[N - 1];
            public int Energy; // энергия
        }

        public double T; // температура
        public double Delta; // разница энергий
        public double P; // вероятность допуска
        public bool fNew; // флаг нового решения
        public bool fBest; // флаг лучшего решения
        public long Time; // этап поиска
        public long Step; // шаг на этапе поиска
        public long Accepted; // число новых решений

        // Меняет в пути последовательность(города меняются местами), модификация решения
        void Swap(TMember M) {
            int x, y, v;

            x = (new Random().Next(0, N - 1));
            y = (new Random().Next(0, N - 1));

            v = M.Plan[x];

            M.Plan[x] = M.Plan[y];
            M.Plan[y] = v;
        }

        // Формирует начальное решение, которое будет меняться на каждой итерации и вспомогательный
        // массив с расстояниями между городов
        public void New(TMember M) {
            Random rand = new Random();

            for (int j = 0; j < N; j++) {
                for (int i = 0; i < N; i++) {
                    if (i == j)
                        RoadWeights[i, j] = 0;
                    else
                        if (j > i)
                        RoadWeights[i, j] = RoadWeights[j, i];
                    else
                        RoadWeights[i, j] = rand.Next(1, 50);

                    Console.Write(" " + RoadWeights[i, j]);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            M.Plan = new int[N - 1];

            for (int i = 0; i < N - 1; i++) {
                M.Plan[i] = i + 1;
            }
        }

        // Расчет энергии текущего решения
        void CalcEnergy(TMember M) {
            int energy = RoadWeights[0, M.Plan[0]];

            for (int i = 0; i < N - 2; i++) {
                energy += RoadWeights[M.Plan[i], M.Plan[i + 1]];
            }

            energy += RoadWeights[M.Plan[N - 2], 0];
            M.Energy = energy;
        }

        // Копирование решения, из текущего в рабочее решение или из текущего в лучшее решение
        void Copy(TMember MD, TMember MS) {
            for (int i = 0; i < N - 1; i++) {
                MD.Plan[i] = MS.Plan[i];
            }

            MD.Energy = MS.Energy;
        }

        // Выводит лучшее решение на экран
        void Show(TMember M) {
            Console.ForegroundColor = ConsoleColor.Green;
            int x, y;
            Console.Write(0 + " -> ");

            for (y = 0; y < N - 1; y++) {
                Console.Write(M.Plan[y] + " -> ");
            }

            Console.Write(0);
            Console.WriteLine();
        }

        void Work() {
            var rand = new Random();
            T = Tn;
            fBest = false;
            Time = 0;
            TMember Current = new TMember(); // текущее решение
            TMember Working = new TMember(); // рабочее решение
            TMember Best = new TMember(); // лучшее решение
            Best.Energy = 999899889;

            New(Current);
            CalcEnergy(Current);
            Copy(Working, Current);

            while (T > Tk) {
                Accepted = 0;

                for (int Step = 0; Step <= IterationsNumber; Step++) {
                    fNew = false;
                    Swap(Working);
                    CalcEnergy(Working);

                    if (Working.Energy <= Current.Energy) {
                        fNew = true;
                    } else {
                        Delta = Working.Energy - Current.Energy;
                        P = Math.Exp(-Delta / T);

                        if (P > rand.NextDouble()) {
                            Accepted = Accepted + 1;
                            fNew = true;
                        }
                    }

                    if (fNew) {
                        fNew = false;
                        Copy(Current, Working);

                        if (Current.Energy < Best.Energy) {
                            Copy(Best, Current);
                            fBest = true;
                        }
                    } else {
                        Copy(Working, Current);
                    }
                }

                Console.Write("Temp=");
                Console.Write(T);
                Console.Write(" Energy=");
                Console.Write(Best.Energy);
                Console.WriteLine();

                T *= Alfa;
            }

            if (fBest)
                Show(Best);
        }

        static void Main() {
            Program program = new Program();
            program.Work();

            Console.ReadKey();
        }
    }
}
