#define _CRT_SECURE_NO_WARNINGS // для отключения предупреждений безопасности, строки №227 и №316
#include <iostream>
#include <Windows.h> //system("cls")("pause")
#include <fstream> // 16, 17 строки
#include <iomanip> // оформление таблиц
#include <vector> //Хранение в памяти
#include <string> //строки
#include <cstdlib> //Функции обычной C
#include <ctime> // Для времени
#include <algorithm> //inventory.begin(), inventory.end()
using namespace std;
/*Система управления запасами склада, т.е.принятие на склад, выдача, списание по качеству
или по срокам и т.д.Всю информацию о предметах сохранять в файле и ссылаться на пункт
в программе для ее просмотра.*/
long long balance;
ifstream fin;
ofstream fout;
struct Product
{
	int id = 0;
	string name;
	int quantity = 0;
	double price = 0;
	int quality = 0;
	string expiryDate;
	string arrivalDate;
};
vector<Product> inventory;
const string FILENAME = "inventory.txt";
void loadInventory();
void saveInventory();
void addProduct();
void issueProduct();
void writeOffProduct();
void viewInventory();
void checkExpiry();
void generateReport();
int generateId();
int main()
{
	SetConsoleOutputCP(1251);
	SetConsoleCP(1251);
	setlocale(0, "ru");
	srand(time(NULL));
	system("color 0A");
	loadInventory();
	int choice;
	do {
		system("cls");
		cout << "========================================\n";
		cout << "    СИСТЕМА УПРАВЛЕНИЯ СКЛАДОМ\n";
		cout << "========================================\n";
		cout << "1. Принять товар на склад\n";
		cout << "2. Выдать товар со склада\n";
		cout << "3. Списать товар\n";
		cout << "4. Просмотреть все товары\n";
		cout << "5. Проверить сроки годности\n";
		cout << "6. Сформировать отчет\n";
		cout << "7. Сохранить и выйти\n";
		cout << "========================================\n";
		cout << "\nВыберите действие: ";
		cin >> choice;
		cin.ignore();
		switch (choice)
		{
		case 1:
			addProduct();
			break;
		case 2:
			issueProduct();
			break;
		case 3:
			writeOffProduct();
			break;
		case 4:
			viewInventory();
			break;
		case 5:
			checkExpiry();
			break;
		case 6:
			generateReport();
			break;
		case 7:
			saveInventory();
			cout << "\nДанные сохранены. Выход...\n";
			break;
		default:
			cout << "e" << "\n";
			continue;
			system("pause");
		}
	} while (choice != 7);
	return 0;
}
void loadInventory()
{
	ifstream fin(FILENAME);
	if (!fin.is_open())
	{
		cout << "Файл не найден. Будет создан новый!" << "\n";
	}

	Product P;
	while (fin >> P.id >> P.quantity >> P.price >> P.quality)
	{
		fin.ignore();
		getline(fin, P.name);
		getline(fin, P.expiryDate);
		getline(fin, P.arrivalDate);
		inventory.push_back(P);
	}
	fin.close();
	cout << "Загружено: " << inventory.size();
}
void saveInventory()
{
	ofstream fout(FILENAME);
	for (const auto& P : inventory)
	{
		fout << P.id << " " << P.quantity << " " << P.price << " " << P.quality << "\n" << P.name << "\n" << P.expiryDate << "\n" << P.arrivalDate << "\n";
	}
	fout.close();
}
int generateId()
{
	int maxId = 1000;
	for (const auto& P : inventory) {
		if (P.id > maxId) {
			maxId = P.id;
		}
	}
	return maxId + 1;
}
bool isValidDate(int day, int month, int year)
{
	if (year < 2000 || year > 2100) return false;
	if (month < 1 || month > 12) return false;
	if (month == 2)
	{
		bool leapYear = (year % 4 == 0 && year % 100 != 0) || (year % 400 == 0);
		if (day < 1 || day >(leapYear ? 29 : 28)) return false;
	}
	else if (month == 4 || month == 6 || month == 9 || month == 11)
	{
		if (day < 1 || day > 30) return false;
	}
	else 
	{
		if (day < 1 || day > 31) return false;
	}
	return true;
}
void addProduct()
{
	system("cls");
	cout << "========================================\n";
	cout << "        ПРИЕМ ТОВАРА НА СКЛАД\n";
	cout << "========================================\n\n";
	Product newProduct;
	newProduct.id = generateId();
	cout << "ID: " << newProduct.id << endl;
	cout << "Название товара: ";
	getline(cin, newProduct.name);
	while (true)
	{
		cout << "Кол-во: ";
		cin >> newProduct.quantity;
		if (cin.fail() || newProduct.quantity < 0) {
			cout << "Ошибка: введите неотрицательное число!\n";
			cin.clear();
			cin.ignore(1000, '\n');
		}
		else {
			break;
		}
	}
	while (true) {
		cout << "Цена: ";
		cin >> newProduct.price;
		if (cin.fail() || newProduct.price < 0) {
			cout << "Ошибка: введите неотрицательное число!\n";
			cin.clear();
			cin.ignore(1000, '\n');
		}
		else {
			break;
		}
	}
	while (true) {
		cout << "Качество (1-5): ";
		cin >> newProduct.quality;
		if (cin.fail() || newProduct.quality < 1 || newProduct.quality > 5)
		{
			cout << "Ошибка: качество должно быть от 1 до 5!\n";
			cin.clear();
			cin.ignore(1000, '\n');
		}
		else {
			break;
		}
	}
	cin.ignore(); // очистка буфера
	bool validDate = false;
	while (!validDate) {
		cout << "Срок годности (ДД.ММ.ГГГГ): ";
		getline(cin, newProduct.expiryDate);
		int day, month, year;
		if (sscanf(newProduct.expiryDate.c_str(), "%d.%d.%d", &day, &month, &year) != 3) {
			cout << "Ошибка: неверный формат! Используйте ДД.ММ.ГГГГ (например: 31.12.2024)\n";
			continue;
		}
		if (!isValidDate(day, month, year)) {
			cout << "Ошибка: неверная дата! Проверьте день, месяц и год.\n";
			cout << "Пример правильной даты: 15.03.2024\n";
			continue;
		}
		time_t now = time(0);
		tm* current = localtime(&now);
		int currentYear = 1900 + current->tm_year;
		int currentMonth = 1 + current->tm_mon;
		int currentDay = current->tm_mday;

		if (year < currentYear ||
			(year == currentYear && month < currentMonth) ||
			(year == currentYear && month == currentMonth && day < currentDay)) {
			cout << "Ошибка: срок годности не может быть в прошлом!\n";
			cout << "Сегодня: " << currentDay << "." << currentMonth << "." << currentYear << endl;
			continue;
		}
		validDate = true;
	}
	time_t now = time(0);
	tm* ltm = localtime(&now);
	char buffer[11];
	strftime(buffer, 11, "%d.%m.%Y", ltm);
	newProduct.arrivalDate = buffer;

	inventory.push_back(newProduct);
	cout << "\nТовар успешно добавлен!\n";
	system("pause");
}
void issueProduct()
{
	system("cls");
	cout << "========================================\n";
	cout << "        ВЫДАЧА ТОВАРА СО СКЛАДА\n";
	cout << "========================================\n\n";
	int id, quantity;
	cout << "ID: ";
	cin >> id;
	while (true) {
		cout << "Кол-во выдачи: ";
		cin >> quantity;
		if (cin.fail() || quantity <= 0) {
			cout << "Ошибка: количество должно быть больше 0!\n";
			cin.clear();
			cin.ignore(1000, '\n');
		}
		else {
			break;
		}
	}
	for (auto it = inventory.begin(); it != inventory.end(); ++it)
	{
		if (it->id == id)
		{
			string productName = it->name;

			if (it->quantity >= quantity)
			{
				it->quantity -= quantity;

				if (it->quantity == 0)
				{
					inventory.erase(it);
					cout << "Товар '" << productName << "' полностью выдан и удален!\n";
				}
				else
				{
					cout << "Выдано " << quantity << " единиц товара '" << productName
						<< "'. Осталось: " << it->quantity << "\n";
				}
			}
			else
			{
				cout << "Недостаточно товара '" << productName
					<< "'! Доступно: " << it->quantity << "\n";
			}
			system("pause");
			return;
		}
	}
	cout << "Товар с ID " << id << " не найден!\n";
	system("pause");
}
void writeOffProduct()
{
	system("cls");
	cout << "========================================\n";
	cout << "           СПИСАНИЕ ТОВАРА\n";
	cout << "========================================\n\n";
	int id, reason;
	cout << "ID: ";
	cin >> id;
	while (true) {
		cout << "\nПричина списания:\n";
		cout << "1. Истек срок годности\n";
		cout << "2. Низкое качество\n";
		cout << "3. Повреждение\n";
		cout << "4. Другое\n";
		cout << "Выберите (1-4): ";
		cin >> reason;
		if (cin.fail() || reason < 1 || reason > 4) {
			cout << "Ошибка: выберите от 1 до 4!\n";
			cin.clear();
			cin.ignore(1000, '\n');
		}
		else {
			break;
		}
	}
	for (auto it = inventory.begin(); it != inventory.end(); ++it)
	{
		if (it->id == id)
		{
			int quantity;
			while (true) {
				cout << "Товар: " << it->name << endl;
				cout << "Доступное количество: " << it->quantity << endl;
				cout << "Сколько списать? ";
				cin >> quantity;

				if (cin.fail() || quantity <= 0) {
					cout << "Ошибка: количество должно быть больше 0!\n";
					cin.clear();
					cin.ignore(1000, '\n');
				}
				else if (quantity > it->quantity) {
					cout << "Нельзя списать больше чем есть на складе!\n";
					cout << "Доступно: " << it->quantity << "\n";
				}
				else {
					break;
				}
			}
			string productName = it->name;
			it->quantity -= quantity;
			ofstream log("write_off_log.txt", ios::app);
			time_t now = time(0);
			char dt[26];
			ctime_s(dt, sizeof(dt), &now);
			log << dt << " | ID: " << id << " | Товар: " << productName
				<< " | Кол-во: " << quantity << " | Причина: " << reason << endl;
			log.close();
			if (it->quantity == 0)
			{
				inventory.erase(it);
				cout << "Товар '" << productName << "' полностью списан и удален.\n";
			}
			else
			{
				cout << "Списано " << quantity << " единиц товара '" << productName
					<< "'. Осталось: " << it->quantity << "\n";
			}

			system("pause");
			return;
		}
	}
	cout << "Товар с ID " << id << " не найден!\n";
	system("pause");
}
void viewInventory()
{
	system("cls");
	cout << "================================================================================\n";
	cout << "                        ВСЕ ТОВАРЫ НА СКЛАДЕ\n";
	cout << "================================================================================\n";
	cout << " ID   Название                     Кол-во   Цена    Кач-во   Срок годности\n";
	cout << "------------------------------------------------------------------------\n";
	if (inventory.empty())
	{
		cout << "                          Нет товаров на складе                             \n";
	}
	else
	{
		for (const auto& P : inventory)
		{

			printf("%4d  %-28s  %6d  %6.2f  %6d  %-13s\n",
				P.id, P.name.c_str(), P.quantity, P.price, P.quality, P.expiryDate.c_str());
		}
	}
	cout << "================================================================================\n";
	double totalValue = 0;
	for (const auto& P : inventory)
	{
		totalValue += P.quantity * P.price;
	}
	std::cout << "\nОбщая стоимость запасов: " << fixed << setprecision(2) << totalValue << " руб.\n";
	system("pause");
}
void checkExpiry()
{
	system("cls");
	cout << "========================================\n";
	cout << "     ПРОВЕРКА СРОКОВ ГОДНОСТИ\n";
	cout << "========================================\n\n";
	time_t now = time(0);
	tm* ltm = localtime(&now);
	int currentYear = 1900 + ltm->tm_year;
	int currentMonth = 1 + ltm->tm_mon;
	int currentDay = ltm->tm_mday;
	bool found = false;
	cout << "Товары с истекшим сроком годности:\n";
	cout << "------------------------------------\n";
	for (const auto& P : inventory) //const auto& - работает как "прозрачное окно" в элемент.
	{
		int day, month, year;
		sscanf(P.expiryDate.c_str(), "%d.%d.%d", &day, &month, &year);
		if (year < currentYear || (year == currentYear && month < currentMonth) || (year == currentYear && month == currentMonth && day < currentDay))
		{
			cout << "ID: " << P.id << " | " << P.name << " | Срок: " << P.expiryDate << " | Кол-во: " << P.quantity << endl;
			found = true;
		}
	}
	if (!found)
	{
		cout << "Нет товаров с истекшим сроком годности.\n";
	}
	system("pause");
}
void generateReport()
{
	system("cls");
	cout << "========================================\n";
	cout << "          ОТЧЕТ ПО СКЛАДУ\n";
	cout << "========================================\n\n";
	ofstream report("склад_отчет.txt");
	report << "ОТЧЕТ ПО СКЛАДУ\n";
	report << "Дата формирования: ";
	time_t now = time(0);
	char* dt = ctime(&now);
	report << dt << "\n";
	report << "=====================================\n\n";
	report << "Всего товаров: " << inventory.size() << "\n\n";
	vector<int> qualityCount(6, 0);
	for (const auto& P : inventory)
	{
		if (P.quality >= 1 && P.quality <= 5)
		{
			qualityCount[P.quality]++;
		}
	}
	report << "Распределение по качеству:\n";
	for (int i = 1; i <= 5; i++)
	{
		report << "Качество " << i << ": " << qualityCount[i] << " товаров\n";
	}
	double TotalValue = 0;
	for (const auto& P : inventory)
	{
		report << P.id << "\t" << P.name << "\t" << P.quantity << "\t" << P.price << "\t" << P.quality << "\t\t" << P.expiryDate << "\n";
	}
	report.close();
	cout << "Отчет сохранен в файл 'склад_отчет.txt'\n";
	system("pause");
}
// report - это объект для записи в файл.
