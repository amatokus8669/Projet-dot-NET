﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Calculatrice
{

    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private bool darkColorTheme;
        public bool backFromPopup { get; set; }

        private string[] simpleEntries = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", ",", "-", "+", "/", "(", ")", "^", "!" };

        private Key[] validKeyslist = { Key.NumPad0, Key.NumPad1, Key.NumPad2, Key.NumPad3, Key.NumPad4, Key.NumPad5, Key.NumPad6, Key.NumPad7, Key.NumPad8, Key.NumPad9, Key.Divide, Key.Multiply, Key.Subtract, Key.Add, Key.Return, Key.Decimal, Key.D5, Key.OemOpenBrackets, Key.S, Key.C, Key.R, Key.E, Key.L, Key.T, Key.Oem8, Key.Oem6 };

        private ObservableCollection<Operation> _listeOperations;
        public ObservableCollection<Operation> ListeOperations
        {
            get
            {
                return _listeOperations;
            }
            set
            {
                _listeOperations = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ListeOperations)));
            }
        }

        private string _saisie;
        public string Saisie
        {
            get { return _saisie; }
            set
            {
                _saisie = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Saisie)));
            }
        }


        public MainWindow()
        {
            ListeOperations = new ObservableCollection<Operation>();
            this.DataContext = this;
            setColorsNightMode();
            InitializeComponent();
            backFromPopup = false;
        }




        #region TouchesClavier

        #region Events
        private void Event_KeyUp(object sender, KeyEventArgs e)
        {
            if (string.IsNullOrEmpty(Saisie))
            {
                Saisie = "";
            }
            TranslateKeyPress(e.Key);
            //Console.WriteLine(e.Key.ToString());
        }

        private void Event_KeyDown(object sender, KeyEventArgs e)
        {
            if (!string.IsNullOrEmpty(Saisie) && e.Key.Equals(Key.Back))
            {
                Erase();
            }
        }
        #endregion

        #region Util
        private void TranslateKeyPress(Key key)
        {
            int index;
            for (index = 0; index < validKeyslist.Count(); index++)
            {
                if (key == validKeyslist[index])
                {
                    break;
                }
            }
            if (index <= 9)
            {
                if (Saisie.Length > 0 && Saisie.ElementAt(Saisie.Length - 1) == ')')
                {
                    Saisie += "*";
                }
                Saisie += index.ToString();
            }
            switch (index)
            {
                case 10:
                    Saisie += "/";
                    break;
                case 11:
                    Saisie += "*";
                    break;
                case 12:
                    Saisie += "-";
                    break;
                case 13:
                    Saisie += "+";
                    break;
                case 14:
                    if (backFromPopup)
                    {
                        backFromPopup = false;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(Saisie))
                        {
                            if (operationIsValid(Saisie))
                            {
                                try
                                {
                                    Operation o = new Operation(Saisie);
                                    ListeOperations.Add(o);
                                    Saisie = "";
                                    UpdateScrollBar(historyBox);
                                }
                                catch (Exception ex)
                                {

                                    Console.WriteLine(ex.ToString());
                                    MessageBox.Show("Erreur de Calcul");
                                    backFromPopup = true;
                                }

                            }
                            else
                            {
                                MessageBox.Show("Formule invalide");
                                backFromPopup = true;
                                Saisie = "";
                            }
                        }
                        else
                        {
                            Saisie = "";
                        }
                    }
                    break;
                case 15:
                    Saisie += ",";
                    break;
                case 16:
                    if (Saisie.Length > 0 && !(isAnOperator(Saisie.ElementAt(Saisie.Length - 1)) || Saisie.ElementAt(Saisie.Length - 1) == '('))
                    {
                        Saisie += "*";
                    }
                    Saisie += "(";
                    break;
                case 17:
                    Saisie += ")";
                    break;
                case 18:
                    if (Saisie.Length > 0 && !(isAnOperator(Saisie.ElementAt(Saisie.Length - 1)) || Saisie.ElementAt(Saisie.Length - 1) == '('))
                    {
                        Saisie += "*";
                    }
                    Saisie += "sin(";
                    break;
                case 19:
                    if (Saisie.Length > 0 && !(isAnOperator(Saisie.ElementAt(Saisie.Length - 1)) || Saisie.ElementAt(Saisie.Length - 1) == '('))
                    {
                        Saisie += "*";
                    }
                    Saisie += "cos(";
                    break;
                case 20:
                    if (Saisie.Length > 0 && !(isAnOperator(Saisie.ElementAt(Saisie.Length - 1)) || Saisie.ElementAt(Saisie.Length - 1) == '('))
                    {
                        Saisie += "*";
                    }
                    Saisie += "√(";
                    break;
                case 21:
                    if (Saisie.Length > 0 && !(isAnOperator(Saisie.ElementAt(Saisie.Length - 1)) || Saisie.ElementAt(Saisie.Length - 1) == '('))
                    {
                        Saisie += "*";
                    }
                    Saisie += "exp(";
                    break;
                case 22:
                    if (Saisie.Length > 0 && !(isAnOperator(Saisie.ElementAt(Saisie.Length - 1)) || Saisie.ElementAt(Saisie.Length - 1) == '('))
                    {
                        Saisie += "*";
                    }
                    Saisie += "ln(";
                    break;
                case 23:
                    if (Saisie.Length > 0 && !(isAnOperator(Saisie.ElementAt(Saisie.Length - 1)) || Saisie.ElementAt(Saisie.Length - 1) == '('))
                    {
                        Saisie += "*";
                    }
                    Saisie += "tan(";
                    break;
                case 24:
                    Saisie += "!";
                    break;
                case 25:
                    Saisie += "^";
                    break;
                default:
                    Saisie += "";
                    break;
            }
        }
        #endregion

        #endregion

        #region TouchesInterface
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button sourceButton = (Button)sender;
            resetFocus(sourceButton);
            string buttonContent = sourceButton.Content.ToString();
            if (string.IsNullOrEmpty(Saisie))
            {
                Saisie = "";
            }
            if (simpleEntries.Contains(buttonContent))
            {

                if (isANumber(buttonContent.ElementAt(0)) && Saisie.Length > 0 && Saisie.ElementAt(Saisie.Length - 1) == ')')
                {
                    Saisie += "*";
                }

                Saisie += buttonContent;
            }
            else
            {
                if (buttonContent.Equals("Clear"))
                {
                    if (string.IsNullOrEmpty(Saisie))
                    {
                        ListeOperations.Clear();
                    }
                    else
                    {
                        Saisie = "";
                    }
                }
                else if (buttonContent.Equals("x"))
                {
                    Saisie += "*";
                }
                else if (buttonContent.Equals("Rep"))
                {
                    if (ListeOperations.Count != 0)
                    {
                        Saisie += ListeOperations[ListeOperations.Count - 1].Resultat;
                    }

                }
                else if (buttonContent.Equals("="))
                {
                    if (!string.IsNullOrEmpty(Saisie))
                    {
                        if (operationIsValid(Saisie))
                        {
                            try
                            {
                                Operation o = new Operation(Saisie);
                                ListeOperations.Add(o);
                                Saisie = "";
                                UpdateScrollBar(historyBox);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.ToString());
                                MessageBox.Show("Erreur de Calcul");
                                backFromPopup = true;
                            }
                        }
                        else
                        {
                            Saisie = "";
                            MessageBox.Show("Formule invalide");
                            backFromPopup = true;
                        }
                    }
                    else
                    {
                        Saisie = "";
                    }
                }
                else if (buttonContent.Equals("Effacer"))
                {
                    Erase();
                }
                else
                {
                    if (Saisie.Length > 0 && !(isAnOperator(Saisie.ElementAt(Saisie.Length - 1)) || Saisie.ElementAt(Saisie.Length - 1) == '('))
                    {
                        Saisie += "*";
                    }

                    if (buttonContent.Equals("sin (s)"))
                    {
                        Saisie += "sin(";
                    }
                    else if (buttonContent.Equals("cos (c)"))
                    {
                        Saisie += "cos(";
                    }
                    else if (buttonContent.Equals("√ (r)"))
                    {
                        Saisie += "√(";
                    }
                    else if (buttonContent.Equals("exp (e)"))
                    {
                        Saisie += "exp(";
                    }
                    else if (buttonContent.Equals("ln (l)"))
                    {
                        Saisie += "ln(";
                    }
                    else if (buttonContent.Equals("tan (t)"))
                    {
                        Saisie += "tan(";
                    }
                }
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = (ListBox)sender;
            Operation selectedOperation = (Operation)listBox.SelectedItem;
            if (selectedOperation != null)
            {
                Popup popup = new Popup(this, selectedOperation);
                popup.Show();
                Point mouseLocation = PointToScreen(Mouse.GetPosition(this));
                popup.Left = mouseLocation.X;
                popup.Top = mouseLocation.Y - popup.Height;
            }
            listBox.UnselectAll();

        }

        private void ListBox_FocusLost(object sender, RoutedEventArgs e)
        {
            ListBox listBox = (ListBox)sender;
            listBox.UnselectAll();
        }



        private void modebutton_Click(object sender, RoutedEventArgs e)
        {
            changeColorTheme();
            resetFocus((Button)sender);
        }
        #endregion

        #region Util


        private Window GetParentWindow(DependencyObject child)
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null)
            {
                return null;
            }

            Window parent = parentObject as Window;
            if (parent == null)
            {
                return parent;
            }
            else
            {
                return GetParentWindow(parentObject);
            }
        }

        private void resetFocus(FrameworkElement element)
        {
            var scope = FocusManager.GetFocusScope(element);
            FocusManager.SetFocusedElement(scope, null);
            Keyboard.Focus(GetParentWindow(element));
        }

        private bool Erase()
        {
            string operation = Saisie.Replace("sin", "n").Replace("cos", "s").Replace("tan", "n").Replace("exp", "p").Replace("ln", "l");
            int length = Saisie.Length;


            if (length > 0)
            {
                char avantDernierChar = operation.ElementAt(operation.Length - 1);
                if (length > 1)
                {
                    avantDernierChar = operation.ElementAt(operation.Length - 2);
                }
                else
                {
                    Saisie = Saisie.Substring(0, Saisie.Length - 1);
                    return true;
                }

                if (length > 3 && (avantDernierChar == 'n' || avantDernierChar == 's' || avantDernierChar == 'p'))
                {
                    //"sin(" "cos(" "exp("
                    Saisie = Saisie.Substring(0, Saisie.Length - 4);
                    return true;

                }
                else if (length > 2 && avantDernierChar == 'l')
                {
                    //"ln("
                    Saisie = Saisie.Substring(0, Saisie.Length - 3);
                    return true;
                }
                else if (length > 1 && avantDernierChar == '√')
                {
                    //"√("
                    Saisie = Saisie.Substring(0, Saisie.Length - 2);
                    return true;
                }
                else
                {
                    Saisie = Saisie.Substring(0, Saisie.Length - 1);
                    return true;
                }
            }
            return false;
        }



        private bool isAnOperator(char c)
        {
            return (c == '+' || c == '-' || c == '*' || c == '/' || c == '^');
        }

        private bool isANumber(char c)
        {
            Regex regex = new Regex("[0-9]");
            return regex.IsMatch(c.ToString());
        }

        private bool isSpecialOperator(char c)
        {
            return (c == 's' || c == 't' || c == 'n' || c == 'p');
        }

        public bool operationIsValid(string operation)
        {
            //vérification nb de parenthèses
            int cpt = 0;
            foreach (char c in operation)
            {
                if (c == '(')
                {
                    cpt++;
                }
                if (c == ')')
                {
                    cpt--;
                }
            }
            if (cpt < 0)
            {
                return false;
            }
            else
            {
                if (cpt > 0)
                {
                    for (int i = 0; i < cpt; i++)
                    {
                        Saisie += ")";
                        operation += ")";
                    }
                }
            }
            //fin vérification des parenthèses

            //retirer les parenthèses qui ne continnent rien
            while (Saisie.Contains("()"))
            {
                Saisie = Saisie.Replace("()", "");
                operation = operation.Replace("()", "");
            }
            if (String.IsNullOrEmpty(Saisie))
            {
                return false;
            }

            operation = operation.Replace("sin", "n").Replace("cos", "s").Replace("tan", "t").Replace("exp", "p").Replace("ln", "l");
            if (Saisie.ElementAt(0) == '+') //Si "+........"
            {
                Saisie = Saisie.Substring(1);
                operation = Saisie; //retirer +
            }
            char firstChar = operation.ElementAt(0);
            if (firstChar == '/' || firstChar == '*' || firstChar == '!' || firstChar == '.' || firstChar == ',')//si opération commence avec * ou / ou !
            {
                return false; //opération invalide
            }
            else
            {
                if (isANumber(firstChar)) //si premier élément est nombre
                {
                    operation = "+" + operation; // ajouter un + pour simplifier
                }
            }

            if (operation.Length <= 2) //si taille == 1 AVEC le "+" en plus càd que operation = "+"
            {
                if (isANumber(operation.ElementAt(1)))
                {
                    return true;
                }
                return false; //operation invalide
            }

            //char secondChar = operation.ElementAt(1);
            //if (secondChar == '*' ||secondChar == '/' ||secondChar == '!')
            //{ //si élément apres le + de début n'est pas un chiffre
            //    return false; //operation invalide
            //}


            for (int i = 2; i < operation.Length - 1; i++)
            {
                char previousChar = operation.ElementAt(i - 1);
                char c = operation.ElementAt(i);
                char nextChar = operation.ElementAt(i + 1);
                if (c == '+')
                {
                    if (!(previousChar == ')' || isANumber(previousChar) || nextChar == '(' || isANumber(previousChar)))
                    {
                        return false;
                    }
                }
                else if (c == '-')
                {
                    if (!(previousChar == ')' || previousChar == '(' || isANumber(previousChar) || nextChar == '(' || isANumber(previousChar)))
                    {
                        return false;
                    }
                }
                else if (c == '/')
                {
                    if (!(previousChar == ')' || isANumber(previousChar) || nextChar == '(' || isANumber(previousChar)))
                    {
                        return false;
                    }
                }
                else if (c == '*')
                {
                    if (!(previousChar == ')' || isANumber(previousChar) || nextChar == '(' || isANumber(previousChar)))
                    {
                        return false;
                    }
                }
                else if (c == '!')
                {
                    if (!(previousChar == ')' || isANumber(previousChar) || isAnOperator(nextChar)))
                    {
                        return false;
                    }
                }
                else if (c == '^')
                {
                    if (!(previousChar == ')' || isANumber(previousChar) || nextChar == '(' || isANumber(nextChar)))
                    {
                        return false;
                    }
                }
                else if (c == '(')
                {
                    if (!(isAnOperator(previousChar) || isANumber(previousChar) || isANumber(nextChar) || nextChar == '-' || isSpecialOperator(previousChar) || isSpecialOperator(nextChar)))
                    {
                        return false;
                    }
                }
                else if (c == ')')
                {
                    if (!(isAnOperator(nextChar) || isANumber(nextChar) || isANumber(previousChar) || previousChar == ')' || nextChar == ')' || nextChar == '-'))
                    {
                        return false;
                    }
                }
            }
            char lastChar = operation.ElementAt(operation.Length - 1);
            if (!(isANumber(lastChar) || lastChar == ')'))
            {
                return false;
            }
            return true;
        }
        #endregion

        #region Interface

        private void UpdateScrollBar(ListBox listBox)
        {
            if (listBox != null)
            {
                listBox.ScrollIntoView(listBox.Items[listBox.Items.Count - 1]);
            }

        }
        private void setColorsDayMode()
        {

            Application.Current.Resources["windowBackgroundColor"] = Color.FromRgb(200, 200, 200);
            Application.Current.Resources["globalForegroundColor"] = Color.FromRgb(0, 0, 0);
            Application.Current.Resources["normalButtonBackgroundColor"] = Color.FromRgb(220, 220, 220);
            Application.Current.Resources["numericButtonBackgroundColor"] = Color.FromRgb(240, 240, 240);
            darkColorTheme = false;
        }

        private void setColorsNightMode()
        {
            Application.Current.Resources["windowBackgroundColor"] = Color.FromRgb(52, 52, 52);
            Application.Current.Resources["globalForegroundColor"] = Color.FromRgb(255, 255, 255);
            Application.Current.Resources["normalButtonBackgroundColor"] = Color.FromRgb(21, 21, 21);
            Application.Current.Resources["numericButtonBackgroundColor"] = Color.FromRgb(29, 29, 29);
            darkColorTheme = true;

        }

        private void changeColorTheme()
        {
            if (darkColorTheme)
            {
                setColorsDayMode();
            }
            else
            {
                setColorsNightMode();
            }
        }

        #endregion


    }
}
