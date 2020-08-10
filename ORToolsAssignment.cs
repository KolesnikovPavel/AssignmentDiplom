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

/*
 * Ссылка на оригинальный алгоритм задачи о назначениях https://developers.google.com/optimization/assignment/simple_assignment
*/
using System;
using System.Collections.Generic;
using Google.OrTools.Graph;

namespace AssignmentDiplom
{
    class ORToolsAssignment
    {
        //Метод алгоритма распределения
        public static void AssignTasks(int?[,] cost, int rows, int cols, int[] idArray, List<DataArray> signCollection)
        {
            LinearSumAssignment assignment = new LinearSumAssignment();
            for (int worker = 0; worker < rows; worker++)
            {
                for (int task = 0; task < cols; task++)
                {
                    if (cost[worker, task] != null)
                    {
                        assignment.AddArcWithCost(worker, task, Convert.ToInt64(cost[worker, task]));
                    }
                }
            }
            LinearSumAssignment.Status solveStatus = assignment.Solve();
            //Проверка статуса решателя
            //Статус OPTIMAL означает что решатель нашёл решение и распределение возможно
            if (solveStatus == LinearSumAssignment.Status.OPTIMAL)
            {
                for (int i = 0; i < assignment.NumNodes(); i++)
                {
                    if (idArray[assignment.RightMate(i)] != 0)
                    {
                        signCollection.Add(new DataArray(i, idArray[assignment.RightMate(i)]));
                    }
                }
            }
            //Статус INFEASIBLE означает что решатель не нашёл решение и распределение невозможно
            if (solveStatus == LinearSumAssignment.Status.INFEASIBLE)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No assignment is possible.");
                Console.ResetColor();
            }
            //Статуы POSSIBLE_OVERFLOW означает что возможно переполнение решателя
            if (solveStatus == LinearSumAssignment.Status.POSSIBLE_OVERFLOW)
            {
                Console.WriteLine("Some input costs are too large and may cause an integer overflow.");
            }
        }
    }
}