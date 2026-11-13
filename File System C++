#include <Windows.h>
#using <System.dll>
#using <System.Windows.Forms.dll>
#using <System.Drawing.dll>
using namespace System;
using namespace System::IO;
using namespace System::Windows::Forms;
using namespace System::Drawing;
using namespace System::Diagnostics;
public ref class MyForm : public Form
{
private:
	ComboBox^ cmbDrive;
	ListBox^ lstFiles;
	Button^ btnUp;
	Label^ lblInfo;
	ImageList^ images;
	String^ currentPath;
public:
	MyForm()
	{
		this->Text = "File Manager";
		this->Size = Drawing::Size(600, 450);
		this->StartPosition = FormStartPosition::CenterScreen;
		cmbDrive = gcnew ComboBox();
		cmbDrive->Location = Point(10, 10);
		cmbDrive->Width = 120;
		cmbDrive->DropDownStyle = ComboBoxStyle::DropDownList;
		cmbDrive->SelectedIndexChanged += gcnew EventHandler(this, &MyForm::SelectDrive);
		btnUp = gcnew Button();
		btnUp->Text = "On Level Up";
		btnUp->Location = Point(145, 10);
		btnUp->Width = 130;
		btnUp->Click += gcnew EventHandler(this, &MyForm::GoUp);
		images = gcnew ImageList();
		images->ImageSize = Drawing::Size(18, 18);
		images->Images->Add(SystemIcons::WinLogo->ToBitmap());
		images->Images->Add(SystemIcons::Application->ToBitmap());
		lstFiles = gcnew ListBox();
		lstFiles->Location = Point(10, 50);
		lstFiles->Size = Drawing::Size(560, 310);
		lstFiles->DrawMode = DrawMode::OwnerDrawFixed;
		lstFiles->ItemHeight = 23;
		lstFiles->Anchor = AnchorStyles::Top | AnchorStyles::Bottom | AnchorStyles::Left | AnchorStyles::Right;
		lstFiles->DrawItem += gcnew DrawItemEventHandler(this, &MyForm::DrawFiles);
		lblInfo = gcnew Label();
		lblInfo->Text = "Information for site";
		lblInfo->Location = Point(10, 375);
		lblInfo->Size = Drawing::Size(560, 30);
		this->Controls->Add(cmbDrive);
		this->Controls->Add(btnUp);
		this->Controls->Add(lstFiles);
		this->Controls->Add(lblInfo);
		for each (DriveInfo ^ drive in DriveInfo::GetDrives())
			if (drive->IsReady)
				cmbDrive->Items->Add(drive->Name);
		if (cmbDrive->Items->Count > 0)
			cmbDrive->SelectedIndex = 0;
	}
	void SelectDrive(Object^ sender, EventArgs^ e)
	{
		currentPath = cmbDrive->SelectedItem->ToString();
		LoadFiles();
	}
	void LoadFiles()
	{
		lstFiles->Items->Clear();
		try
		{
			DirectoryInfo^ dir = gcnew DirectoryInfo(currentPath);
			for each (DirectoryInfo ^ folder in dir->GetDirectories())
				lstFiles->Items->Add("Folder|" + folder->Name);
			for each (FileInfo ^ file in dir->GetFiles())
				lstFiles->Items->Add("File|" + file->Name);
		}
		catch(Exception^)
		{
			MessageBox::Show("Error to folder");
		}
	}
	void DrawFiles(Object^ senter, DrawItemEventArgs^ e)
	{
		if (e->Index < 0)
			return;
		e->DrawBackground();
		String^ item = lstFiles->Items[e->Index]->ToString();
		int icon = item->StartsWith("Folder|") ? 0 : 1;
		images->Draw(e->Graphics, e->Bounds.X + 3, e->Bounds.Y + 2, icon);
		String^ name = item->Substring(item->IndexOf('|') + 1);
		e->Graphics->DrawString(name, e->Font, gcnew SolidBrush(e->ForeColor), (float)e->Bounds.X + 27, (float)e->Bounds.Y + 3);
		e->DrawFocusRectangle();
	}
	void OpenItem(Object^ sender, EventArgs^ e)
	{
		if (lstFiles->SelectedItem == nullptr)
			return;
		String^ item = lstFiles->SelectedItem->ToString();
		String^ name = item->Substring(item->IndexOf('|') + 1);
		String^ path = Path::Combine(currentPath, name);
		try
		{
			if (item->StartsWith("Folder|"))
			{
				currentPath = path;
				LoadFiles();
			}
			else
			{
				Process::Start(path);
			}
		}
		catch (Exception^)
		{
			MessageBox::Show("Error opened file");
		}
	}
	void GoUp(Object^ sender, EventArgs^ e)
	{
		DirectoryInfo^ dir = gcnew DirectoryInfo(currentPath);
		if (dir->Parent != nullptr)
		{
			currentPath = dir->Parent->FullName;
			LoadFiles();
		}
	}
	void ShowInfo(Object^ sender, EventArgs^ e)
	{
		if (lstFiles->SelectedItem == nullptr)
			return;
		String^ item = lstFiles->SelectedItem->ToString();
		String^ name = item->Substring(item->IndexOf('|') + 1);
		String^ path = Path::Combine(currentPath, name);
		try
		{
			if (item->StartsWith("File|"))
			{
				FileInfo^ file = gcnew FileInfo(path);
				lblInfo->Text = "Size: " + file->Length + " Bite | Created: " + file->CreationTime.ToString();
			}
			else
			{
				DirectoryInfo^ folder = gcnew DirectoryInfo(path);
				lblInfo->Text = "Folder | Created: " + folder->CreationTime.ToString();
			}
		}
		catch (Exception^)
		{
			lblInfo->Text = "Information to no states";
		}
	}
};
[STAThreadAttribute]
int main(array<String^>^ args)
{
	Application::EnableVisualStyles();
	Application::SetCompatibleTextRenderingDefault(false);
	Application::Run(gcnew MyForm());
	return 0;
}
