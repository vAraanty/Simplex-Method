using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Simplex_Method
{
    public partial class Form1 : Form
    {
        List<TextBox> columnTextBoxes = new List<TextBox>();
        List<TextBox> rowTextBoxes = new List<TextBox>();
        List<List<TextBox>> grid = new List<List<TextBox>>();
        List<TextBox> unitsBoxes = new List<TextBox>();
        List<List<Label>> unitsGrid = new List<List<Label>>();
        List<ComboBox> constraintsParam = new List<ComboBox>();
        List<ComboBox> constraintsSymb = new List<ComboBox>();
        List<TextBox> constraintsGrid = new List<TextBox>();

        public Form1()
        {
            InitializeComponent();
        }
        // Функція для додавання стовпцю до програмного інтерфейса.
        // У поля, що додаються фунцією, користувач вписує дані задачі,
        // у шапку стовпця - назву виробу, продукт тощо.
        private void AddColumn()
        {
            int columns = columnTextBoxes.Count;
            if (columns >= 6)
            {
                return;
            }
            TextBox textBox = new TextBox() { Left = 150 + 120 * columns, Top = 20, Text = "Запаси" };
            textBox.Leave += TextBox_Leave;
            columnTextBoxes.Add(textBox);
            Controls.Add(textBox);
            for (int i = 1; i < rowTextBoxes.Count; i++)
            {
                grid[i][columns].Visible = true;
                if (columns == 0)
                {
                    unitsBoxes[i].Visible = true;
                }
                else
                {
                    unitsGrid[i][columns - 1].Visible = true;
                }
            }
            if (columns > 0)
            {
                columnTextBoxes[columns - 1].Text = $"Стовпець {columns}";
                grid[0][columns - 1].Visible = true;
                if (columns > 1)
                {
                    unitsGrid[0][columns - 2].Visible = true;
                }
                else
                {
                    unitsBoxes[0].Visible = true;
                }
            }

            TextBox_Leave(null, null);
        }
        // Функція для видалення стовпця з програмного інтерфейса.
        private void RemoveColumn()
        {
            int columns = columnTextBoxes.Count;
            if (columns <= 3)
            {
                return;
            }
            var lastTextBox = columnTextBoxes.Last();
            Controls.Remove(lastTextBox);
            columnTextBoxes.RemoveAt(columns - 1);
            for (int i = 0; i < rowTextBoxes.Count; i++)
            {
                grid[i][columns - 1].Visible = false;
                grid[i][columns - 1].Clear();

                unitsGrid[i][columns - 2].Visible = false;
            }
            if (columns > 0)
            {
                columnTextBoxes[columns - 2].Text = "Запаси";
                grid[0][columns - 2].Visible = false;
                unitsGrid[0][columns - 3].Visible = false;
            }

            TextBox_Leave(null, null);
        }
        // Функція для додавання рядку до програмного інтерфейса.
        // У поля, що додаються фунцією, користувач вписує дані задачі,
        // у шапку рядку - назву матеріалу, ресурсу тощо.

        private void AddRow()
        {
            int rows = rowTextBoxes.Count;
            if (rows >= 6)
            {
                return;
            }
            TextBox textBox = new TextBox() { Left = 20, Top = 60 + 40 * rows, Text = $"Рядок {rows}" };
            rowTextBoxes.Add(textBox);
            Controls.Add(textBox);
            for (int i = 0; i < columnTextBoxes.Count; i++)
            {
                grid[rows][i].Visible = true;
                if (i == 0)
                {
                    unitsBoxes[rows].Visible = true;
                }
                else
                {
                    unitsGrid[rows][i - 1].Visible = true;
                }
            }
        }
        // Функція для видалення рядку з програмного інтерфейса.
        private void RemoveRow()
        {
            int rows = rowTextBoxes.Count;
            if (rows <= 3)
            {
                return;
            }
            var lastTextBox = rowTextBoxes.Last();
            Controls.Remove(lastTextBox);
            rowTextBoxes.RemoveAt(rows - 1);
            for (int i = 0; i < grid[0].Count; i++)
            {
                grid[rows - 1][i].Visible = false;
                grid[rows - 1][i].Clear();

                if (i == 0)
                {
                    unitsBoxes[rows - 1].Visible = false;
                    unitsBoxes[rows - 1].Clear();
                }
                else
                {
                    unitsGrid[rows - 1][i - 1].Visible = false;
                    unitsGrid[rows - 1][i - 1].Text = string.Empty;
                }
            }

        }
        // Функція що додає функціональні кнопки, використовуючі які,
        // користувач може додавати рядки, стовпці, обмеження
        private Button CreateGridControls(int top, int left, string text, EventHandler AddButton_Click, EventHandler RemoveButton_Click)
        {
            Label label = new Label() { Top = top, Left = left, Text = text };
            label.AutoSize = true;
            Controls.Add(label);

            Button addButton = new Button() { Top = label.Top + label.Height + 5, Left = label.Left, Text = "+", Width = 25 };
            addButton.Click += AddButton_Click;
            Controls.Add(addButton);

            Button removeButton = new Button() { Top = addButton.Top, Left = addButton.Left + addButton.Width + 5, Text = "-", Width = addButton.Width };
            removeButton.Click += RemoveButton_Click;
            Controls.Add(removeButton);

            return removeButton;
        }
        // Функція, що заповнює усі поля одиниць вимірювання даними,
        // що були введені у початкові поля одиниць вимірювання
        private void FillUnitsLabels(TextBox sender)
        {
            int index = unitsBoxes.IndexOf(sender);
            for (int i = 0; i < unitsGrid[0].Count; i++)
            {
                unitsGrid[index][i].Text = sender.Text;
            }
        }
        // Функція, що початково створює обмеження, які
        // користувач може додавати або видаляти
        private void CreateConstraints()
        {
            for (int i = 0; i < 5; i++)
            {
                ComboBox constraintParam = new ComboBox() { Left = 320, Top = 310 + 23 * i, Visible = false };
                constraintParam.DataSource = columnTextBoxes.Where(x => x != columnTextBoxes.Last()).ToList();
                constraintParam.DisplayMember = "Text";
                Controls.Add(constraintParam);
                constraintsParam.Add(constraintParam);

                ComboBox constraintSymb = new ComboBox() { Left = constraintParam.Left + constraintParam.Width + 5, Top = 310 + 23 * i, Width = 50, Visible = false };
                constraintSymb.DataSource = new List<string> { " <=", " >=" };
                Controls.Add(constraintSymb);
                constraintsSymb.Add(constraintSymb);

                TextBox constraintTextBox = new TextBox() { Left = constraintSymb.Left + constraintSymb.Width + 5, Top = 310 + 23 * i, Visible = false };
                constraintTextBox.TextChanged += GridTextBox_TextChanged;
                constraintTextBox.KeyPress += GridTextBox_KeyPress;
                Controls.Add(constraintTextBox);
                constraintsGrid.Add(constraintTextBox);
            }
        }
        // Функція для додавання обмежень до програмного інтерфейса.
        // У полях, що додаються фунцією, користувач обирає стовпець, для
        // якого буде створено обмеження, знак обмеження, та число обмеження
        private void AddConstraint()
        {
            int constraints = constraintsGrid.Where(x => x.Visible == true).Count();
            if (constraints >= 5)
            {
                return;
            }
            constraintsGrid[constraints].Visible = true;
            constraintsParam[constraints].Visible = true;
            constraintsSymb[constraints].Visible = true;
        }
        // Функція, що видаляє обмеження з програмного інтерфейса.
        private void RemoveConstraint()
        {
            int constraints = constraintsGrid.Where(x => x.Visible == true).Count();
            if (constraints == 0)
            {
                return;
            }
            constraintsGrid[constraints - 1].Visible = false;
            constraintsParam[constraints - 1].Visible = false;
            constraintsSymb[constraints - 1].Visible = false;
        }
        // Функція, що перевіряє правильність, введених у поля, даних.
        // Рядки для чисел приймають лише цифри(0-9) та кому й крапку.
        // Рядки не можуть бути порожніми.
        // Якщо поле не відповідає критеріям воно виділяється червоним
        // кольором, та подальша робота програми не відбувається
        private bool ValidateFields()
        {
            bool flag = true;

            for (int i = 0; i < rowTextBoxes.Count; i++)
            {
                rowTextBoxes[i].BackColor = rowTextBoxes[i].Text == "" ? Color.OrangeRed : Color.White;
                if (rowTextBoxes[i].Text == "")
                    flag = false;

                unitsBoxes[i].BackColor = unitsBoxes[i].Text == "" ? Color.OrangeRed : Color.White;
                if (unitsBoxes[i].Text == "")
                    flag = false;

                for (int j = 0; j < columnTextBoxes.Count; j++)
                {
                    if (i == 0)
                    {
                        columnTextBoxes[j].BackColor = columnTextBoxes[j].Text == "" ? Color.OrangeRed : Color.White;
                        if (columnTextBoxes[j].Text == "")
                            flag = false;
                    }

                    if (j != columnTextBoxes.Count - 1)
                    {
                        grid[i][j].BackColor = grid[i][j].Text == "" ? Color.OrangeRed : Color.White;
                        if (grid[i][j].Text == "")
                            flag = false;
                        grid[i][j].Text = grid[i][j].Text.Replace('.', ',');
                    }
                }
            }

            for (int i = 0; i < constraintsGrid.Where(x => x.Visible == true).Count(); i++)
            {
                constraintsGrid[i].BackColor = constraintsGrid[i].Text == "" ? Color.OrangeRed : Color.White;
                if (constraintsGrid[i].Text == "")
                    flag = false;
            }

            Console.WriteLine(flag);
            return flag;
        }
        // Функція, що за наданими користувачем даними, вирішує задачу
        // симплекс методом.
        private void simplexMethod()
        {
            float M = Int16.MaxValue;

            // Формування списку коефіцієнтів Z
            List<float> zCoefs = new List<float>();

            for (int i = 0; i < columnTextBoxes.Count - 1; i++)
            {
                zCoefs.Add(float.Parse(grid[0][i].Text));
            }

            int constraintCount = constraintsGrid.Where(x => x.Visible == true).Count();
            int artificialCount = constraintsSymb.Where(x => x.SelectedIndex == 1).Count();
            for (int i = 0; i < rowTextBoxes.Count - 1 + constraintCount; i++)
            {
                zCoefs.Add(0.0f);
            }
            for (int i = 0; i < artificialCount; i++)
            {
                zCoefs.Add(-M);
            }

            // Формування списку усіх змінних
            List<int> variables = new List<int>();
            for (int i = 0; i < zCoefs.Count; i++)
            {
                variables.Add(i);
            }

            // Формування базових змінних задачі
            List<int> basic = new List<int>(variables.Where(x => x >= columnTextBoxes.Count - 1 && x < variables.Count - artificialCount * 2));
            for (int i = zCoefs.Count - artificialCount; i < zCoefs.Count; i++)
            {
                basic.Add(i);
            }

            // Формування таблиці зі значеннями та стовпця X0
            List<List<float>> table = new List<List<float>>();
            List<float> X0 = new List<float>();
            for (int i = 1; i < rowTextBoxes.Count; i++)
            {
                table.Add(new List<float>());
                for (int j = 0; j < columnTextBoxes.Count - 1; j++)
                {
                    table[i - 1].Add(float.Parse(grid[i][j].Text));
                }

                X0.Add(float.Parse(grid[i][columnTextBoxes.Count - 1].Text));
            }
            for (int i = 0; i < table.Count; i++)
            {
                for (int j = 0; j < basic.Count + artificialCount; j++)
                {
                    table[i].Add(i == j ? 1.0f : 0.0f);
                }
            }

            for (int i = 0; i < constraintCount; i++)
            {
                System.Diagnostics.Debug.WriteLine($"{constraintsParam[i].SelectedIndex} {constraintsSymb[i].SelectedIndex} {constraintsGrid[i].Text}");
                table.Add(new List<float>());
                for (int j = 0; j < columnTextBoxes.Count - 1; j++)
                {
                    table[table.Count - 1].Add(j == constraintsParam[i].SelectedIndex ? 1.0f : 0.0f);
                }

                X0.Add(float.Parse(constraintsGrid[i].Text));
            }
            for (int i = table.Count - constraintCount; i < table.Count; i++)
            {
                for (int j = 0; j < basic.Count - constraintCount; j++)
                {
                    table[i].Add(0.0f);
                }
                for (int j = 0; j < constraintCount; j++)
                {
                    table[i].Add(j != i - table.Count + constraintCount ? 0.0f : constraintsSymb[i - table.Count + constraintCount].SelectedIndex == 0 ? 1.0f : -1.0f);
                }
                for (int j = 0; j < artificialCount; j++)
                {
                    table[i].Add(j != i - table.Count + constraintCount ? 0.0f : 1.0f);
                }
            }

            // Формування коефіцієнтів при m+1
            List<float> m1Coefs = new List<float>();
            for (int i = 0; i < table[0].Count; i++)
            {
                float result = 0;
                for (int j = 0; j < table.Count; j++)
                {
                    result += zCoefs[basic[j]] * table[j][i];
                }
                result -= zCoefs[i];
                m1Coefs.Add(result);
            }

            float temp = 0.0f;
            for (int i = 0; i < basic.Count; i++)
            {
                temp += zCoefs[basic[i]] * X0[i];
            }
            X0.Add(temp);

            // --------------------- PRINT TABLE --------------------------

            const int spaces = 12;
            System.Diagnostics.Debug.Write($"{"",spaces}");
            System.Diagnostics.Debug.Write($"{"",spaces}");
            for (int i = 0; i < zCoefs.Count; i++)
            {
                System.Diagnostics.Debug.Write($"{zCoefs[i],spaces}");
            }
            System.Diagnostics.Debug.WriteLine("");
            for (int i = 0; i < table.Count; i++)
            {
                System.Diagnostics.Debug.Write($"{basic[i],spaces}");
                System.Diagnostics.Debug.Write($"{X0[i],spaces}");
                for (int j = 0; j < table[i].Count; j++)
                {
                    System.Diagnostics.Debug.Write($"{table[i][j],spaces}");
                }
                System.Diagnostics.Debug.WriteLine("");
            }
            System.Diagnostics.Debug.Write($"{"",spaces}");
            System.Diagnostics.Debug.Write($"{X0.Last(),spaces}");
            for (int i = 0; i < m1Coefs.Count; i++)
            {
                System.Diagnostics.Debug.Write($"{m1Coefs[i],spaces}");
            }
            System.Diagnostics.Debug.WriteLine("");
            System.Diagnostics.Debug.WriteLine("");

            // -------------------- END PRINTING --------------------------


            while (true)
            {
                // Знаходження стовпчика, в якому m+1 приймає найменше значення
                float lowestColumn = m1Coefs.Min();

                // Якщо найменше значення m+1 невід'ємне - то задача вирішена
                // і потрібно вивести користувачу відомості про отримані результати
                if (lowestColumn >= 0)
                {
                    // Знаходження значення Z при отриманих результатах
                    float bestZ = 0;
                    for (int i = 0; i < basic.Count; i++)
                    {
                        bestZ += zCoefs[basic[i]] * X0[i];
                    }

                    // Розрахунок, скільки матеріалів було використано 
                    // в отриманому оптимальному рішенні
                    List<float> usedMaterials = new List<float>();
                    for (int i = 0; i < columnTextBoxes.Count - 1; i++)
                    {
                        int index = basic.IndexOf(i);
                        if (index != -1)
                        {
                            usedMaterials.Add(X0[index]);
                        }
                        else
                        {
                            usedMaterials.Add(0.0f);
                        }
                    }

                    // Формування тексту з відомостями про отримані результати
                    string messageText = $"Найбільший прибуток: { bestZ} {unitsBoxes[0].Text}\n" + "\nДля отримання прибутку потрібно виробити:\n";
                    for (int i = 0; i < columnTextBoxes.Count - 1; i++)
                    {
                        messageText += $"{columnTextBoxes[i].Text}: {System.Math.Round(usedMaterials[i])} шт.\n";
                    }
                    messageText += "\nПри такому плані буде використано:\n";
                    string remainingResourses = "\nЗалишок ресурсів:\n";
                    for (int i = 1; i < rowTextBoxes.Count; i++)
                    {
                        float sum = 0;
                        for (int j = 0; j < columnTextBoxes.Count - 1; j++)
                        {
                            sum += float.Parse(grid[i][j].Text) * usedMaterials[j];
                        }
                        messageText += $"{rowTextBoxes[i].Text}: {System.Math.Round(sum)} {unitsBoxes[i].Text}\n";
                        remainingResourses += $"{ rowTextBoxes[i].Text}: {System.Math.Round(float.Parse(grid[i][columnTextBoxes.Count - 1].Text) - sum)} {unitsBoxes[i].Text}\n";
                    }
                    messageText += remainingResourses;

                    // Виведення відомостей користувачу
                    MessageBox.Show(messageText, "Рішення");

                    return;
                }
                int indexOfLowestColumn = m1Coefs.IndexOf(lowestColumn);

                // Знаходження рядку, в якому значення Q є найменшим
                float lowestRow = float.MaxValue;
                int indexOfLowestRow = -1;
                for (int i = 0; i < table.Count; i++)
                {
                    float divisionResult = X0[i] / table[i][indexOfLowestColumn];
                    if (divisionResult < 0)
                    {
                        continue;
                    }
                    if (divisionResult < lowestRow)
                    {
                        lowestRow = divisionResult;
                        indexOfLowestRow = i;
                    }
                }

                basic[indexOfLowestRow] = variables[indexOfLowestColumn];
                X0[indexOfLowestRow] = lowestRow;

                // Отриманя елементу, що знаходиться на перетині стовпчика з найменшим значенням m+1
                // та рядку з найменшим значенням Q
                // Відбувається ділення рядку на отриманий елемент, для отримання одиниці у перетині
                float divideBy = table[indexOfLowestRow][indexOfLowestColumn];
                for (int i = 0; i < table[0].Count; i++)
                {
                    table[indexOfLowestRow][i] /= divideBy;
                }

                // Відбувається приведення стовпчика вигляду, коли значення в усіх рядках, 
                // окрім мінімального, приймають 0.
                // Отримання нової таблиці
                divideBy = table[indexOfLowestRow][indexOfLowestColumn];
                for (int i = 0; i < table.Count; i++)
                {
                    if (i == indexOfLowestRow)
                    {
                        continue;
                    }

                    float multiplyBy = table[i][indexOfLowestColumn] / divideBy;
                    X0[i] -= X0[indexOfLowestRow] * multiplyBy;
                    for (int j = 0; j < table[i].Count; j++)
                    {
                        table[i][j] -= table[indexOfLowestRow][j] * multiplyBy;
                    }
                }

                float m1MultiplyBy = m1Coefs[indexOfLowestColumn] / divideBy;
                for (int i = 0; i < m1Coefs.Count; i++)
                {
                    m1Coefs[i] -= table[indexOfLowestRow][i] * m1MultiplyBy;
                }
                X0[X0.Count - 1] -= X0[indexOfLowestRow] * m1MultiplyBy;

                // --------------------- PRINT TABLE --------------------------

                // const int spaces = 12;

                System.Diagnostics.Debug.Write($"{"",spaces}");
                System.Diagnostics.Debug.Write($"{"",spaces}");
                for (int i = 0; i < zCoefs.Count; i++)
                {
                    System.Diagnostics.Debug.Write($"{zCoefs[i],spaces}");
                }
                System.Diagnostics.Debug.WriteLine("");
                for (int i = 0; i < table.Count; i++)
                {
                    System.Diagnostics.Debug.Write($"{basic[i],spaces}");
                    System.Diagnostics.Debug.Write($"{X0[i],spaces}");
                    for (int j = 0; j < table[i].Count; j++)
                    {
                        System.Diagnostics.Debug.Write($"{table[i][j],spaces}");
                    }
                    System.Diagnostics.Debug.WriteLine("");
                }
                System.Diagnostics.Debug.Write($"{"",spaces}");
                System.Diagnostics.Debug.Write($"{X0.Last(),spaces}");
                for (int i = 0; i < m1Coefs.Count; i++)
                {
                    System.Diagnostics.Debug.Write($"{m1Coefs[i],spaces}");
                }
                System.Diagnostics.Debug.WriteLine("");
                System.Diagnostics.Debug.WriteLine("");

                // -------------------- END PRINTING --------------------------


            }
        }

        // Запуск програмного додатку, та формування графічного інтерфейсу додатку
        // заповнення додатку початковими даними, рядками, стовпцями, кнопками.
        private void Form1_Load(object sender, EventArgs e)
        {
            // Створення кнопок для керування кількістю рядків
            // стовпців, обмежень
            var lastButton = CreateGridControls(290, 20, "Рядки", AddRowButton_Click, RemoveRowButton_Click);
            lastButton = CreateGridControls(lastButton.Top + lastButton.Height + 5, 20, "Стовпці", AddColumnButton_Click, RemoveColumnButton_Click);
            CreateGridControls(lastButton.Top + lastButton.Height + 5, 20, "Обмеження", AddConstraintButton_Click, RemoveConstraintButton_Click);

            Label constraintsLabel = new Label() { Top = 290, Left = this.Width / 2, Text = "Обмеження", AutoSize = true };
            Controls.Add(constraintsLabel);

            for (int i = 0; i < 6; i++)
            {
                grid.Add(new List<TextBox>());
                unitsGrid.Add(new List<Label>());
                for (int j = 0; j < 6; j++)
                {
                    TextBox gridTextBox = new TextBox() { Left = 150 + 120 * j, Top = 60 + 40 * i, Width = 55, Visible = false };
                    gridTextBox.TextChanged += GridTextBox_TextChanged;
                    gridTextBox.KeyPress += GridTextBox_KeyPress;
                    Controls.Add(gridTextBox);
                    grid[i].Add(gridTextBox);

                    if (j != 0)
                    {
                        Label unitLabel = new Label();
                        unitLabel.Top = gridTextBox.Top;
                        unitLabel.Left = gridTextBox.Left + gridTextBox.Width + 5;
                        unitLabel.Width = 40;
                        unitLabel.Visible = false;
                        Controls.Add(unitLabel);
                        unitsGrid[i].Add(unitLabel);
                    }
                }

                TextBox unitTextBox = new TextBox() { Left = 210, Top = 60 + 40 * i, Width = 40, Visible = false };
                unitTextBox.Leave += UnitTextBox_Leave;
                unitsBoxes.Add(unitTextBox);
                Controls.Add(unitTextBox);


            }


            AddColumn();
            AddColumn();
            AddColumn();
            AddRow();
            AddRow();
            AddRow();
            rowTextBoxes[0].Text = "Ціна";
            grid[0][columnTextBoxes.Count - 1].Visible = false;
            unitsGrid[0][columnTextBoxes.Count - 1].Visible = false;

            CreateConstraints();

            Button submitButton = new Button() { Left = 700, Top = 350, Text = "Розрахувати", AutoSize = true };
            submitButton.Click += SubmitButton_Click;
            Controls.Add(submitButton);
        }
        // Перевірка введених у поле користувачем даних
        private void GridTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.') && (e.KeyChar != ','))
            {
                e.Handled = true;
            }
        }
        private void GridTextBox_TextChanged(object sender, EventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch((sender as TextBox).Text, "[^0-9.,]"))
            {
                (sender as TextBox).Text = "";
            }
        }
        // Додавання стовпця при натисканні кнопки додавання стовпця
        private void AddColumnButton_Click(object sender, EventArgs e)
        {
            AddColumn();
        }
        // Додавання рядку при натисканні кнопки додавання рядку
        private void AddRowButton_Click(object sender, EventArgs e)
        {
            AddRow();
        }
        // Видалення стовпця при натисканні кнопки видалення стовпця
        private void RemoveColumnButton_Click(object sender, EventArgs e)
        {
            RemoveColumn();
        }
        // Видалення рядку при натисканні кнопки видалення рядку
        private void RemoveRowButton_Click(object sender, EventArgs e)
        {
            RemoveRow();
        }
        // Додавання обмеження при натисканні кнопки додавання обмеження
        private void AddConstraintButton_Click(object sender, EventArgs e)
        {
            AddConstraint();
        }
        // Видалення обмеження при натисканні кнопки видалення обмеження
        private void RemoveConstraintButton_Click(object sender, EventArgs e)
        {
            RemoveConstraint();
        }
        // Заповнення усього рядку одиницями вимірювання, введеними користувачем у поле
        private void UnitTextBox_Leave(object sender, EventArgs e)
        {
            FillUnitsLabels(sender as TextBox);
        }
        // Оновлення даних про назви стовпців
        private void TextBox_Leave(object sender, EventArgs e)
        {
            for (int i = 0; i < constraintsGrid.Count; i++)
            {
                constraintsParam[i].DataSource = columnTextBoxes.Where(x => x != columnTextBoxes.Last()).ToList();
            }
        }
        // Перевірка введених користувачем даних
        // якщо дані корректні, виконання розрахунку
        // за допомогою симплекс методу
        private void SubmitButton_Click(object sender, EventArgs e)
        {
            if (!ValidateFields())
            {
                return;
            }
            simplexMethod();
        }

    }

}
