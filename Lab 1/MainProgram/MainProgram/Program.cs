using System;
using System.Collections.Generic;

namespace MainProgram {
    public class Program {
        const int N = 30; // размер доски и число ферзей 
        const double Tn = 30.0; // начальная температура 
        const double Tk = 0.5; // конечная температура 
        const double Alfa = 0.98; // скорость охлаждения 
        const int ST = 100; // число итераций при смене T

        public class TMember {
            public int[] Plan = new int[N];
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

        // Модификация решения
        void Swap(TMember M) {
            int x, y, v;
            x = (new Random().Next(0, N - 1));

            do {
                y = (new Random().Next(0, N - 1));
            } while (x == y);

            v = M.Plan[x];
            M.Plan[x] = M.Plan[y];
            M.Plan[y] = v;
        }

        // Формирует начальное решение, которое будет меняться на каждой итерации
        public void New(TMember M) {
            for (int i = 0; i < N; i++) {
                M.Plan[i] = i;
            }

            for (int i = 0; i < N; i++) {
                Swap(M);
            }
        }

        // Расчет энергии текущего решения
        void CalcEnergy(TMember M)
        { // расчет энергии
            List<int> dx = new List<int>() { -1, 1, -1, 1 };
            List<int> dy = new List<int>() { -1, 1, 1, -1 };

            int j, x, tx, ty;
            int error = 0;

            for (x = 0; x < N; x++) {
                for (j = 0; j < 4; j++) {
                    tx = x + dx[j];
                    ty = M.Plan[x] + dy[j];
                    while (tx >= 0 && tx < N && ty >= 0 && ty < N) {
                        if (M.Plan[tx] == ty) {
                            error++;
                        }

                        tx = tx + dx[j];
                        ty = ty + dy[j];
                    }
                }
            }
            M.Energy = error;
        }

        // Копирование решения, из текущего в рабочее решение или из текущего в лучшее решение
        void Copy(TMember MD, TMember MS) {
            for (int i = 0; i < N; i++) {
                MD.Plan[i] = MS.Plan[i];
            }

            MD.Energy = MS.Energy;
        }

        // Выводит лучшее решение на экран
        void Show(TMember M){ 
            int x, y;
            Console.WriteLine("Решение:");

            for (y = 0; y < N; y++) {
                for (x = 0; x < N; x++) {
                    if (M.Plan[x] == y)
                        Console.Write("Q");
                    else
                        Console.Write(".");
                }
                Console.WriteLine();
            }
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

            Best.Energy = 999999999;

            New(Current);
            CalcEnergy(Working);
            Copy(Working, Current);

            while (T > Tk) {
                Accepted = 0;

                for (int Step = 0; Step <= ST; Step++) {
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
                T = T * Alfa;
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
