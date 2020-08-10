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

using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace AssignmentDiplom
{
    //КЛАССЫ МОДЕЛИ
    //Класс InspectionContext необходим для считывания базы данных
    public class InspectionContext : DbContext
    {
        //Inspections представляет собой таблицу Inspections в базе данных
        public DbSet<Inspection> Inspections { get; set; }
        //Workers представляет собой таблицу Workers в базе данных
        public DbSet<Worker> Workers { get; set; }
        //Метод конфигурации пути до места расположения базы данных на диске
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite(@"Data Source=C:\Users\Pavel\source\repos\AssignmentDiplom\Inspection.db");
    }

    //Класс Inspection определяет логическую структуру таблицы Inspections
    public class Inspection
    {
        //Id - идентификационный номер объекта
        public int Id { get; set; }
        //Classifier - классификатор объекта
        public string Classifier { get; set; }
        //Date - дата проведение осмотра
        public string Date { get; set; }
        //Time - длительность осмотра
        public int Time { get; set; }
        //Worker - имя работника назначенного на осмотр
        public string Worker { get; set; }
    }

    //Класс Worker определяет логическую структуру таблицы Workers
    public class Worker
    {
        //Id - идентификационный номер работника
        public int Id { get; set; }
        //Name - имя работника
        public string Name { get; set; }
        //Specialization - специализация работника
        public string Specialization { get; set; }
        //Preferences - особые предпочтения работника
        public string Preferences { get; set; }
        //UsedTime - загруженность работника
        public int UsedTime { get; set; }
        //Status - текущее состояние работника
        public string Status { get; set; }
    }
}