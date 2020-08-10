/*********
Министерство науки и высшего образования Российской Федерации
ФГАОУ ВО «УрФУ имени первого Президента России Б.Н. Ельцина»
Институт Радиоэлектроники и информационных технологий - РТФ
Департамент информационных технологий

Разработано в ходе работы над ВКР на тему 
Построение маршрута с минимальными затратами ресурсов и максимальным эффектом 

Студентом
Колесниковым П.Д.
Направление подготовки Управление в технических системах

Данный документ является интеллектуальной собственностью. 
Охраняется в соответствии с Законом о защите прав интеллектуальной собственности. 
Любое копирование и несанкционированное использование документа без согласия автора ЗАПРЕЩЕНО.
*******/

using System;
using System.Collections.Generic;
using System.Linq;

namespace AssignmentDiplom
{
    class Program
    {
        //Метод считывающий базу данных и заполняющий типизированные списки задач и работников
        public static void ReadDB(List<Inspection> tasksCollection, List<Worker> workersCollection)
        {
            using (InspectionContext db = new InspectionContext())
            {
                var inspectionsDB = db.Inspections.ToList();
                foreach (var inspection in inspectionsDB)
                {
                    tasksCollection.Add(inspection);
                }
                var workersDB = db.Workers.Where(worker => worker.Status == "доступен").ToList();
                foreach (var worker in workersDB)
                {
                    workersCollection.Add(worker);
                }
            }
        }

        //Метод получения задач на текущий день
        public static void GetCurrentDayTasks(List<Inspection> tasksCollection, DateTime startDate, int day, List<Inspection> dailyTasksCollection)
        {
            foreach (var e in tasksCollection)
            {
                if (e.Date == startDate.AddDays(day).ToString().Remove(10))
                {
                    dailyTasksCollection.Add(e);
                }
            }
        }

        //Метод удаляющий лишние задачи для осмотра
        public static void RemoveExtraTasks(List<Inspection> tasksCollection, List<Inspection> dailyTasksCollection, List<int> idCollection)
        {
            if (idCollection.Count != 0)
            {
                foreach (var e in tasksCollection)
                {
                    foreach (var a in idCollection)
                    {
                        if (e.Id == a)
                        {
                            dailyTasksCollection.Remove(e);
                        }
                    }
                }
            }
        }

        //Метод удаляющий столбцы-пустышки
        public static void RemoveDummyColumn(List<Inspection> dailyTasksCollection, Inspection dummyTask)
        {
            int counter = 0;
            foreach (var e in dailyTasksCollection)
            {
                if (e == dummyTask)
                {
                    counter++;
                }
            }
            for (int i = 0; i < counter; i++)
            {
                dailyTasksCollection.Remove(dummyTask);
            }
        }

        //Метод добавляющий столбец-пустышку
        public static void AddDummyColumn(List<Inspection> dailyTasksCollection, Inspection dummyTask, int costCols)
        {
            while (dailyTasksCollection.Count < costCols)
            {
                dailyTasksCollection.Add(dummyTask);
            }
        }

        //Метод проверяющий возможность выполнения определенным сотрудником конкретной задачи
        public static bool TaskImpossible(List<Inspection> dailyTasksCollection, int task, List<Worker> workersCollection, int worker)
        {
            return dailyTasksCollection[task].Time + workersCollection[worker].UsedTime >= 8 && dailyTasksCollection[task].Time != 10;
        }

        //Метод получения массива идентификационных номеров
        public static int[] GetIdArray(List<Inspection> dailyTasksCollection, int idArrayLength, int[]idArray)
        {
            for (int task = 0; task < idArrayLength; task++)
            {
                idArray[task] = dailyTasksCollection[task].Id;
            }
            return idArray;
        }

        //Метод подсчитывающий загруженность (в часах) работника
        public static void CountUsedTime(List<DataArray> signsCollection, List<Inspection> dailyTasksCollection, List<Worker> workersCollection)
        {
            foreach (var e in signsCollection)
            {
                for (int i = 0; i < dailyTasksCollection.Count; i++)
                {
                    if (e.Id == dailyTasksCollection[i].Id)
                    {
                        workersCollection[e.Worker].UsedTime += dailyTasksCollection[i].Time;
                    }
                }
            }
        }

        //Метод записи имени работника к соответствующему распределенному объекту 
        public static void AssignWorker(List<DataArray> signsCollection, List<Worker> workersCollection)
        {
            foreach (var e in signsCollection)
            {
                Console.WriteLine("Worker: " + workersCollection[e.Worker].Name + " assignet to task: " + e.Id + " Time used: " + workersCollection[e.Worker].UsedTime);
                using (InspectionContext db = new InspectionContext())
                {
                    var inspectionsDB = db.Inspections.ToList();
                    var workersDB = db.Workers.ToList();
                    //workersDB[e.Worker].UsedTime += workersCollection[e.Worker].UsedTime;
                    inspectionsDB[e.Id - 1].Worker = workersCollection[e.Worker].Name;
                    db.SaveChanges();
                }
            }
        }

        //Точка входа приложения 
        static void Main()
        {
            //Типизированный список в который будут записаны все объекты из подключаемой базы данных
            var tasksCollection = new List<Inspection>();
            //Типизированный список объектов из базы данных необходимых для определенного дня
            var dailyTasksCollection = new List<Inspection>();
            //Типизированный список распределённых объектов из ежедневного листа
            var signsCollection = new List<DataArray>();
            //Типизированный список работников в который будут записаны данные о доступных работниках
            var workersCollection = new List<Worker>();
            //Типизированный список идентификационных номеров распределённых объектов
            var idCollection = new List<int>();

            //Размерность матрицы стоимостей
            int costRows;
            int costCols;

            //Размерность массива идентификационных номеров
            int idArrayLength;

            //Вызов метода считывания данных из базы данных
            ReadDB(tasksCollection, workersCollection);

            //Создание нового объекта-пустышку класса Inspection
            var dummyTask = new Inspection
            {
                Time = 10
            };

            //Задание размерности матрицы стоимостей
            costRows = costCols = workersCollection.Count;

            //Задание размерности массива идентификационных номеров
            idArrayLength = workersCollection.Count;

            DateTime startDate = new DateTime(2019, 10, 01);
            DateTime endDate = new DateTime(2019, 10, 31);
            TimeSpan diff = endDate.Subtract(startDate);
            int totalDays = Int32.Parse(diff.ToString().Remove(diff.ToString().IndexOf(".")));
            
            //Цикл дней
            for (int day = 0; day <= totalDays; day++)
            {
                //Обнуление загруженности работника
                foreach (var e in workersCollection)
                {
                    e.UsedTime = 0;
                }
                Console.WriteLine("\nDate: " + startDate.AddDays(day).ToString().Remove(10));
                GetCurrentDayTasks(tasksCollection, startDate, day, dailyTasksCollection);
                //iterationNum - количество циклов распределения которые необходимо сделать для полного распределения
                for (double iterationNum = Math.Ceiling(dailyTasksCollection.Count / 4.0); iterationNum != 0; iterationNum--)
                {
                    RemoveExtraTasks(tasksCollection, dailyTasksCollection, idCollection);
                    RemoveDummyColumn(dailyTasksCollection, dummyTask);
                    AddDummyColumn(dailyTasksCollection, dummyTask, costCols);
                    //Матрица стоимостей
                    int?[,] cost = new int?[costRows, costCols];
                    //Массив идентификационных номеров
                    int[] idArray = new int[idArrayLength];
                    //Заполнение матрицы стоимостей
                    for (int worker = 0; worker < costRows; worker++)
                    {
                        for (int task = 0; task < costCols; task++)
                        {
                            if (TaskImpossible(dailyTasksCollection, task, workersCollection, worker))
                            {
                                cost[worker, task] = null;
                            }
                            else
                            {
                                cost[worker, task] = dailyTasksCollection[task].Time + workersCollection[worker].UsedTime;
                            }
                        }
                    }
                    //Заполнение массива идентификационных номеров
                    idArray = GetIdArray(dailyTasksCollection, idArrayLength, idArray);
                    //Вызов алгоритма распределения
                    ORToolsAssignment.AssignTasks(cost, costRows, costCols, idArray, signsCollection);
                    CountUsedTime(signsCollection, dailyTasksCollection, workersCollection);
                    //Заполнение типизированного списка идентификационных номеров распределённых объектов
                    foreach (var e in signsCollection)
                    {
                        idCollection.Add(e.Id);
                    }
                    AssignWorker(signsCollection, workersCollection);
                    signsCollection.Clear();
                }
            }
        }
    }
}