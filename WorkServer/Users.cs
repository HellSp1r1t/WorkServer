using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml;

namespace WorkServer
{
    public class Users
    {
        //Список пользователей строкой
        public static bool AuthUser(string login, string password)
        {
            bool auth = false;

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load("Users.xml");

            foreach (XmlElement element in xmlDocument.GetElementsByTagName("element"))
            {
                if (element.GetAttribute("UserLogin").Equals(login) && element.GetAttribute("UserPassword").Equals(password))
                {
                    auth = true;
                }
            }

            return auth;
        }
        //Проверка авторизации
        public static string GetAllUsers() {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load("Users.xml");

            XmlNodeList nodeList = xmlDocument.GetElementsByTagName("element");

            string[] usersList = new string[nodeList.Count];

            int i = 0;
            foreach (XmlElement element in nodeList)
            {
                usersList[i] = element.GetAttribute("UserLogin");
                i++;
            }

            return string.Join(",", usersList);
        }
        //Добаваление нового пользователя
        public static void AddNewUser(string login, string password) {
            //Открываем документ
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load("Users.xml");
            //Создаем элемент
            XmlNode element = xmlDocument.CreateElement("element");
            //Добавляем элемент как дочерный к документу
            xmlDocument.DocumentElement.AppendChild(element);
            //Добавляем атрибут login
            XmlAttribute loginAttribute = xmlDocument.CreateAttribute("UserLogin");
            loginAttribute.Value = login;
            element.Attributes.Append(loginAttribute);
            //Добавляем атрибут пароль
            XmlAttribute passwordAttribute = xmlDocument.CreateAttribute("UserPassword");
            passwordAttribute.Value = password;
            element.Attributes.Append(passwordAttribute);
            //Сохраняем
            xmlDocument.Save("Users.xml");
        }
    }
}
