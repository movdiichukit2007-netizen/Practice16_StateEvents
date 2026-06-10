using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace Practice16_StateEvents;

public class MainForm : Form
{
    private readonly Label lblDisplay = new();
    private readonly TableLayoutPanel tableButtons = new();
    private readonly RadioButton radioOn = new();
    private readonly RadioButton radioOff = new();
    private Button? btnSeven;

    private double firstNumber;
    private string operation = "";
    private bool isOperationSelected;

    public MainForm()
    {
        Text = "Калькулятор";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(460, 560);
        Size = new Size(460, 600);
        BackColor = Color.FromArgb(245, 245, 245);
        KeyPreview = true;

        Panel pnlDisplay = new()
        {
            Dock = DockStyle.Top,
            Height = 92,
            Padding = new Padding(14, 14, 14, 6)
        };

        lblDisplay.Name = "lblDisplay";
        lblDisplay.Text = "0";
        lblDisplay.Dock = DockStyle.Fill;
        lblDisplay.BackColor = Color.WhiteSmoke;
        lblDisplay.ForeColor = Color.Black;
        lblDisplay.TextAlign = ContentAlignment.MiddleRight;
        lblDisplay.Font = new Font("Segoe UI", 26, FontStyle.Regular);
        lblDisplay.BorderStyle = BorderStyle.FixedSingle;
        lblDisplay.TextChanged += LblDisplay_TextChanged;
        pnlDisplay.Controls.Add(lblDisplay);

        Panel pnlState = new()
        {
            Dock = DockStyle.Top,
            Height = 36,
            Padding = new Padding(14, 0, 14, 0)
        };

        radioOn.Name = "radioOn";
        radioOn.Text = "ON";
        radioOn.ForeColor = Color.Green;
        radioOn.Checked = true;
        radioOn.AutoSize = true;
        radioOn.Location = new Point(16, 7);
        radioOn.CheckedChanged += RadioState_CheckedChanged;

        radioOff.Name = "radioOff";
        radioOff.Text = "OFF";
        radioOff.ForeColor = Color.Red;
        radioOff.AutoSize = true;
        radioOff.Location = new Point(72, 7);
        radioOff.CheckedChanged += RadioState_CheckedChanged;

        pnlState.Controls.Add(radioOn);
        pnlState.Controls.Add(radioOff);

        tableButtons.Dock = DockStyle.Fill;
        tableButtons.ColumnCount = 4;
        tableButtons.RowCount = 5;
        tableButtons.Padding = new Padding(14, 10, 14, 14);
        tableButtons.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;

        for (int i = 0; i < 4; i++)
        {
            tableButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        }

        for (int i = 0; i < 5; i++)
        {
            tableButtons.RowStyles.Add(new RowStyle(SizeType.Percent, 20));
        }

        Button btnCe = AddButton("CE", 0, 0, Clear_Click);
        btnCe.ForeColor = Color.OrangeRed;
        AddButton("←", 1, 0, Backspace_Click);
        AddButton("%", 2, 0, Percent_Click);
        AddButton("/", 3, 0, Operation_Click);
        btnSeven = AddButton("7", 0, 1, Number_Click);
        AddButton("8", 1, 1, Number_Click);
        AddButton("9", 2, 1, Number_Click);
        AddButton("*", 3, 1, Operation_Click);
        AddButton("4", 0, 2, Number_Click);
        AddButton("5", 1, 2, Number_Click);
        AddButton("6", 2, 2, Number_Click);
        AddButton("-", 3, 2, Operation_Click);
        AddButton("1", 0, 3, Number_Click);
        AddButton("2", 1, 3, Number_Click);
        AddButton("3", 2, 3, Number_Click);
        AddButton("+", 3, 3, Operation_Click);
        AddButton("0", 0, 4, Number_Click);
        AddButton(".", 1, 4, Number_Click);
        AddButton("=", 2, 4, Equal_Click, 2);

        Controls.Add(tableButtons);
        Controls.Add(pnlState);
        Controls.Add(pnlDisplay);

        KeyDown += Form_KeyDown;
    }

    private Button AddButton(string text, int column, int row, EventHandler handler, int columnSpan = 1)
    {
        Button button = new()
        {
            Text = text,
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 24, FontStyle.Regular),
            Margin = new Padding(5),
            FlatStyle = FlatStyle.Standard,
            BackColor = Color.WhiteSmoke,
            UseVisualStyleBackColor = true
        };

        button.Click += handler;
        tableButtons.Controls.Add(button, column, row);

        if (columnSpan > 1)
        {
            tableButtons.SetColumnSpan(button, columnSpan);
        }

        return button;
    }

    private void RadioState_CheckedChanged(object? sender, EventArgs e)
    {
        if (radioOn.Checked)
        {
            SetCalculatorButtonsEnabled(true);
            KeyPreview = true;
        }
        else if (radioOff.Checked)
        {
            SetCalculatorButtonsEnabled(false);
            KeyPreview = false;
            ClearDisplay(empty: true);
        }
    }

    private void SetCalculatorButtonsEnabled(bool enabled)
    {
        foreach (Control control in tableButtons.Controls)
        {
            control.Enabled = enabled;
        }
    }

    private void LblDisplay_TextChanged(object? sender, EventArgs e)
    {
        int length = lblDisplay.Text.Length;

        if (length <= 3)
        {
            lblDisplay.ForeColor = Color.Black;
        }
        else if (length <= 8)
        {
            lblDisplay.ForeColor = Color.DarkGreen;
        }
        else
        {
            lblDisplay.ForeColor = Color.DarkRed;
        }
    }

    private void Number_Click(object? sender, EventArgs e)
    {
        string symbol = ((Button)sender!).Text;
        AddSymbol(symbol);
    }

    private void AddSymbol(string symbol)
    {
        if (lblDisplay.Text == "0" || isOperationSelected)
        {
            lblDisplay.Text = "";
            isOperationSelected = false;
        }

        if (symbol == "." && lblDisplay.Text.Contains('.'))
        {
            return;
        }

        lblDisplay.Text += symbol;
    }

    private void Operation_Click(object? sender, EventArgs e)
    {
        firstNumber = GetDisplayNumber();
        operation = ((Button)sender!).Text;
        isOperationSelected = true;
    }

    private void Equal_Click(object? sender, EventArgs e)
    {
        double secondNumber = GetDisplayNumber();
        double result;

        switch (operation)
        {
            case "+":
                result = firstNumber + secondNumber;
                break;
            case "-":
                result = firstNumber - secondNumber;
                break;
            case "*":
                result = firstNumber * secondNumber;
                break;
            case "/":
                if (Math.Abs(secondNumber) < double.Epsilon)
                {
                    MessageBox.Show("Ділення на нуль заборонено.", "Помилка");
                    return;
                }

                result = firstNumber / secondNumber;
                break;
            default:
                return;
        }

        lblDisplay.Text = result.ToString(CultureInfo.InvariantCulture);
        operation = "";
        isOperationSelected = true;
    }

    private void Clear_Click(object? sender, EventArgs e)
    {
        ClearDisplay();
    }

    private void ClearDisplay(bool empty = false)
    {
        lblDisplay.Text = empty ? "" : "0";
        firstNumber = 0;
        operation = "";
        isOperationSelected = false;
    }

    private void Backspace_Click(object? sender, EventArgs e)
    {
        if (lblDisplay.Text.Length <= 1)
        {
            lblDisplay.Text = "0";
            return;
        }

        lblDisplay.Text = lblDisplay.Text[..^1];
    }

    private void Percent_Click(object? sender, EventArgs e)
    {
        double value = GetDisplayNumber();
        lblDisplay.Text = (value / 100).ToString(CultureInfo.InvariantCulture);
    }

    private void Form_KeyDown(object? sender, KeyEventArgs e)
    {
        if (!radioOn.Checked)
        {
            return;
        }

        if (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9)
        {
            AddSymbol(((int)(e.KeyCode - Keys.D0)).ToString());
        }
        else if (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9)
        {
            AddSymbol(((int)(e.KeyCode - Keys.NumPad0)).ToString());
        }
        else if (e.KeyCode == Keys.Decimal || e.KeyCode == Keys.OemPeriod)
        {
            AddSymbol(".");
        }
        else if (e.KeyCode == Keys.Add)
        {
            SelectOperation("+");
        }
        else if (e.KeyCode == Keys.Subtract || e.KeyCode == Keys.OemMinus)
        {
            SelectOperation("-");
        }
        else if (e.KeyCode == Keys.Multiply)
        {
            SelectOperation("*");
        }
        else if (e.KeyCode == Keys.Divide)
        {
            SelectOperation("/");
        }
        else if (e.KeyCode == Keys.Enter)
        {
            Equal_Click(this, EventArgs.Empty);
        }
        else if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.Back)
        {
            ClearDisplay();
        }
    }

    private void SelectOperation(string selectedOperation)
    {
        firstNumber = GetDisplayNumber();
        operation = selectedOperation;
        isOperationSelected = true;
    }

    private double GetDisplayNumber()
    {
        return double.TryParse(lblDisplay.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double value)
            ? value
            : 0;
    }

    public void CreateScreenshots(string outputDirectory)
    {
        StartPosition = FormStartPosition.Manual;
        Location = new Point(80, 80);
        Show();
        Application.DoEvents();

        radioOff.Checked = true;
        Application.DoEvents();
        SaveFormImage(Path.Combine(outputDirectory, "practice16_off.png"));

        radioOn.Checked = true;
        lblDisplay.Text = "7654321";
        btnSeven?.Focus();
        Application.DoEvents();
        SaveFormImage(Path.Combine(outputDirectory, "practice16_on_7_symbols.png"));

        Hide();
    }

    private void SaveFormImage(string path)
    {
        using Bitmap bitmap = new(Width, Height);
        DrawToBitmap(bitmap, new Rectangle(0, 0, Width, Height));
        bitmap.Save(path);
    }
}
