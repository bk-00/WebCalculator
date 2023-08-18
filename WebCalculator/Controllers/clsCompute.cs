using System;
using System.Collections.Generic;
using System.Linq;

namespace WebCalculator.Controllers
{
    class clsCompute
    {
        #region Declaration
        private const char opPlus = '+';
        private const char opMinus = '-';
        private const char opMul = '*';
        private const char opDiv = '/';
        #endregion

        #region calculate
        public static double calculate(string sum, out string strMessage)
        {
            strMessage = string.Empty;

            string strtmpBracket = string.Empty;
            double dbltmpResult = 0;
            bool blnExit = false;
            string strMul = "";
            
            try
            {
                //To remove whitespace in expression
                string strtmpExpression = sum.Replace(" ", string.Empty);
                
                //To validate the expression
                if (validateExpression(strtmpExpression, out strMessage))
                {
                    //To solve the expression in bracket first
                    if (strtmpExpression.Contains('(') || strtmpExpression.Contains(')'))
                    {
                        for (int i = strtmpExpression.Length - 1; i > -1; i--)
                        {
                            if (strtmpExpression[i] == '(')
                            {
                                strMul = "";
                                if (i > 0)
                                {
                                    //To append multiply-operator * in-front-of opening-bracket '(' if eg. expression = 1(2)
                                    if (char.IsDigit(strtmpExpression[i - 1]) || strtmpExpression[i - 1] == '.' || strtmpExpression[i - 1] == ')')
                                        strMul += opMul;
                                    //To append number '1' with multiply-operator * (1*) in-front-of opening-bracket '(' when there is operators +, - in-front-of opening-bracket '('
                                    else if (!char.IsDigit(strtmpExpression[i - 1]) && strtmpExpression[i - 1] != '*' && strtmpExpression[i-1] != '/')
                                        strMul += "1" + opMul;
                                }
                                
                                for (int j = i; j < strtmpExpression.Length; j++)
                                {
                                    if (strtmpExpression[j] == ')')
                                    {
                                        strtmpBracket = strtmpExpression.Substring(i + 1, j - i - 1);

                                        //To compute the value in bracket-expression
                                        if (compute(strtmpBracket, out dbltmpResult, out strMessage))
                                        {
                                            //To combine the computed value in bracket-expression with the rest of expression
                                            strtmpExpression = strtmpExpression.Substring(0, i) + strMul + Convert.ToString(dbltmpResult) + strtmpExpression.Substring(j + 1);
                                            i = strtmpExpression.Length - 1;
                                            break;
                                        }
                                        else
                                        {
                                            //To exit loop if unable to compute the value in bracket-expression
                                            blnExit = true;
                                            break;
                                        }
                                    }
                                }
                            }

                            if (blnExit)
                                break;
                        }
                    }

                    //To solve the rest of expression after completely solving the value in bracket-expression
                    if (!blnExit)
                        compute(strtmpExpression, out dbltmpResult, out strMessage);
                }
            }
            catch (Exception ex)
            {
                //throw ex;
                strMessage = ex.Message;
            }
            return dbltmpResult;
        }
        #endregion

        #region validateExpression
        public static bool validateExpression(string strExpression, out string strError)
        {
            bool blnReturn = false;
            strError = string.Empty;

            try
            {
                for (int i=0; i<strExpression.Length; i++)
                {
                    //To verify the expression does not contain alphabets, and any operators other than +, -, *, /, (, )
                    if (!char.IsDigit(strExpression[i]) && strExpression[i] != opPlus && strExpression[i] != opMinus && strExpression[i] != opMul && strExpression[i] != opDiv && strExpression[i] != '.' && strExpression[i] != '(' && strExpression[i] != ')')
                    {
                        if (char.IsLetter(strExpression[i]))
                            strError = "Expression should not contain alphabets";
                        else
                            strError = "Expression only allow to contain digits, decimal-point, " + opPlus + ", " + opMinus + ", " + opMul + ", " + opDiv + ", (, )";

                        blnReturn = false;
                        break;
                    }
                    else
                        blnReturn = true;

                    //To verify the expression does not contain operators like +*, -*, **, /*, +/, -/, */, //
                    if (strExpression[i] == opMul || strExpression[i] == opDiv)
                    {
                        if (i > 0)
                        {
                            if (strExpression[i - 1] == opPlus || strExpression[i - 1] == opMinus || strExpression[i - 1] == opMul || strExpression[i - 1] == opDiv)
                            {
                                strError = "Syntax Error. Invalid operator ";
                                blnReturn = false;
                                break;
                            }
                            else
                                blnReturn = true;
                        }
                    }
                    //To verify the expression only allow ++, -+, *+, /+, +-, --, *-, /-, (+), (-)
                    //if Positive-value with sign '+' or Negative-value with sign '-', it is allowed to be followed directly after operator +, -, *, / or opening-bracket '(' in expression
                    //if expression with '+++' or '+++++++++' or '---' or '--------', it is not allowed and error will be prompted
                    else if (strExpression[i] == opPlus || strExpression[i] == opMinus)
                    {
                        blnReturn = true;

                        if (i-2 > -1)
                        {
                            if (!char.IsDigit(strExpression[i - 1]) && strExpression[i - 1] != '.' && strExpression[i - 1] != '(' && strExpression[i - 1] != ')')
                            {
                                if (!char.IsDigit(strExpression[i - 2]) && strExpression[i - 2] != '.' && strExpression[i - 2] != '(' && strExpression[i - 2] != ')')
                                {
                                    strError = "Syntax Error. Invalid operator ";
                                    blnReturn = false;
                                    break;
                                }
                            }
                        }
                    }
                    //To verify a number must be followed either before or after a decimal-point
                    //decimal number can start with decimal-point, but a number must be followed after the decimal-point
                    //decimal number can end with decimal-point, but a number must be followed in-front-of the decimal-point
                    else if (strExpression[i] == '.')
                    {
                        blnReturn = true;

                        if (i > 1 && i < strExpression.Length - 1)
                        {
                            if (!char.IsDigit(strExpression[i - 1]) && !char.IsDigit(strExpression[i + 1]))
                            {
                                strError = "A number is expected before or after a decimal-point";
                                blnReturn = false;
                                break;
                            }
                        }
                        else if (i == 0)
                        {
                            strError = "A number is expected before or after a decimal-point";
                            blnReturn = false;

                            if (strExpression.Length > 1)
                            {
                                if (char.IsDigit(strExpression[i + 1]))
                                {
                                    strError = string.Empty;
                                    blnReturn = true;
                                }
                            }

                            if (!blnReturn)
                                break;
                        }
                        else if (i == strExpression.Length - 1)
                        {
                            if (!char.IsDigit(strExpression[i-1]))
                            {
                                strError = "A number is expected before or after a decimal-point";
                                blnReturn = false;
                                break;
                            }
                        }
                    }
                }

                if (blnReturn)
                {
                    //To verify the expression is not started with closing-bracket ')', or operators like *, /
                    if (!char.IsDigit(strExpression[0]) && strExpression[0] != '.' && strExpression[0] != opPlus && strExpression[0] != opMinus && strExpression[0] != '(' && blnReturn)
                    {
                        if (strExpression[0] == ')')
                            strError = "Expression should not start with closing-bracket ')', instead it should start with opening-bracket '('";
                        else
                            strError = "Expression should not start with operator, instead it can start with only number or decimal-point";

                        blnReturn = false;
                    }
                    else
                    {
                        blnReturn = true;
                    }
                }
                  
                if (blnReturn)
                {
                    //To verify the expression is end with opening-bracket '(', or any operators like +, -, * /
                    if (!char.IsDigit(strExpression[strExpression.Length - 1]) && strExpression[strExpression.Length - 1] != '.' && strExpression[strExpression.Length - 1] != ')' && blnReturn)
                    {
                        if (strExpression[strExpression.Length - 1] == '(')
                            strError = "Expression should not end with opening-bracket '(', instead it should end with closing-bracket ')'";
                        else
                            strError = "Expression should not end with operator, instead it can end with only number or decimal-point";

                        blnReturn = false;
                    }
                    else
                    {
                        blnReturn = true;
                    }
                }
                
                if (blnReturn)
                {
                    //To verify the total number of opening- and closing-bracket in the expression is equal
                    if (strExpression.Contains('(') || strExpression.Contains(')'))
                    {
                        int openBracketCount = strExpression.Count(c => c == '(');
                        int closeBracketCount = strExpression.Count(c => c == ')');

                        if (openBracketCount == closeBracketCount)
                        {
                            int count = 0;

                            for (int i = 0; i < strExpression.Length - 1; i++)
                            {
                                //To verify only numbers, decimal-point, or operators like +, - is followed after the opening-bracket '('
                                if (strExpression[i] == '(')
                                {
                                    if (!char.IsDigit(strExpression[i + 1]) && strExpression[i + 1] != '.' && strExpression[i + 1] != '(' && strExpression[i + 1] != opPlus && strExpression[i + 1] != opMinus)
                                    {
                                        strError = "Syntax Error. Invalid mathematical expression";
                                        blnReturn = false;
                                        break;
                                    }
                                }

                                //To verify only number, or decimal-point is followed before the closing-bracket ')'
                                if (strExpression[i] == ')')
                                {
                                    count += 1;

                                    if (!char.IsDigit(strExpression[i - 1]) && strExpression[i - 1] != '.' && strExpression[i - 1] != ')' || char.IsDigit(strExpression[i + 1]))
                                    {
                                        strError = "Syntax Error. Invalid mathematical expression";
                                        blnReturn = false;
                                        break;
                                    }
                                    else
                                    {
                                        blnReturn = true;

                                        if (count == closeBracketCount)
                                            break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            //To prompt error if total number of opening- and closing-bracket is not equal
                            if (openBracketCount < closeBracketCount)
                                strError = "Missing opening-bracket '(' in expression";
                            else
                                strError = "Missing closing-bracket ')' in expression";

                            blnReturn = false;
                        }
                    }
                }

                //if (!blnReturn)
                //    throw new Exception(strError);
            }
            catch (Exception ex)
            {
                //throw ex;
                strError = ex.Message;
            }
            return blnReturn;
        }
        #endregion

        #region compute
        private static bool compute(string strExpression, out double dbltmpResult, out string strError)
        {
            strError = string.Empty;
            List<double> dblNum = new List<double>();
            List<char> chOperator = new List<char>();
            string strtmpNum = string.Empty;
            dbltmpResult = 0;
            bool blnReturn = true;

            try
            {
                //If the expression start with '+' or '-', append '0' for calculation
                if (strExpression[0] == opMinus || strExpression[0] == opPlus)
                    strExpression = '0' + strExpression;
                
                //To seperate the number and operator in expression
                //if a number or decimal number or negative number is detected, store in dblNum
                //if an operator is detected, store in chOperator
                for (int i=0; i<strExpression.Length; i++)
                {
                    if (char.IsDigit(strExpression[i]) || strExpression[i] == '.')
                    {
                        strtmpNum += strExpression[i];
                    }
                    else
                    {
                        chOperator.Add(strExpression[i]);
                        dblNum.Add(Convert.ToDouble(strtmpNum));
                        strtmpNum = string.Empty;

                        //To store the positive-value with sign '+' and negative-value with sign '-' on dblNum
                        if (strExpression[i + 1] == opMinus || strExpression[i+1] == opPlus)
                        {
                            strtmpNum += strExpression[i + 1];
                            i += 1;
                        }
                    }

                    //To verify the decimal number only contain 1 decimal-point
                    if (!string.IsNullOrEmpty(strtmpNum) && strtmpNum.Contains('.'))
                    {
                        if (strtmpNum.Count(c => c == '.') > 1)
                        {
                            strError = "Invalid decimal value in expression";
                            strtmpNum = string.Empty;
                            blnReturn = false;
                            break;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(strtmpNum))
                    dblNum.Add(Convert.ToDouble(strtmpNum));

                //To initialize the computed-result with the first value in dblNum
                //it help to compute or return the expression that consist only values
                if (dblNum.Count > 0)
                    dbltmpResult = dblNum[0];

                if (blnReturn)
                {
                    //To perform multiplication and division first if the expression consist of operator *, /
                    //the multiplication or division will be performed in order
                    if (chOperator.Contains(opMul) || chOperator.Contains(opDiv))
                    {
                        for (int i = 0; i < chOperator.Count; i++)
                        {
                            if (chOperator[i] == opMul || chOperator[i] == opDiv)
                            {
                                if (cal2Num(dblNum[i], dblNum[i + 1], chOperator[i], out dbltmpResult, out strError))
                                {
                                    dblNum[i + 1] = dbltmpResult;
                                    dblNum.RemoveAt(i);
                                    chOperator.RemoveAt(i);
                                    i -= 1;
                                }
                                else
                                {
                                    blnReturn = false;
                                    break;
                                }
                            }
                        }
                    }
                }
                
                if (blnReturn)
                {
                    //To perform addition and substraction after complete multiplication or division if the expression consist of operator +, -
                    //the addition or substraction will be performed in order
                    if (chOperator.Contains(opPlus) || chOperator.Contains(opMinus))
                    {
                        for (int j = 0; j < chOperator.Count; j++)
                        {
                            if (chOperator[j] == opPlus || chOperator[j] == opMinus)
                            {
                                if (cal2Num(dblNum[j], dblNum[j + 1], chOperator[j], out dbltmpResult, out strError))
                                {
                                    dblNum[j + 1] = dbltmpResult;
                                    dblNum.RemoveAt(j);
                                    chOperator.RemoveAt(j);
                                    j -= 1;
                                }
                                else
                                {
                                    blnReturn = false;
                                    break;
                                }
                            }
                        }
                    }
                }

                //if (!string.IsNullOrEmpty(strError))
                //    throw new Exception(strError);
            }
            catch (Exception ex)
            {
                blnReturn = false;
                //throw ex;
                strError = ex.Message;
            }
            finally
            {
                dblNum.Clear();
                dblNum = null;
                chOperator.Clear();
                chOperator = null;
            }
            return blnReturn;
        }
        #endregion

        #region cal2Num
        private static bool cal2Num(double dblNum1, double dblNum2, char op, out double dblResult, out string strError)
        {
            dblResult = 0;
            strError = string.Empty;
            bool blnReturn = true;

            try
            {
                //The addition, substraction, multiplication, or division of numbers will be performed based on operator +, -, *, /
                switch (op)
                {
                    case opPlus:
                        dblResult = dblNum1 + dblNum2;
                        break;
                    case opMinus:
                        dblResult = dblNum1 - dblNum2;
                        break;
                    case opMul:
                        dblResult = dblNum1 * dblNum2;
                        break;
                    case opDiv:
                        //Error will be thrown if the denominator = 0 when perform division
                        if (dblNum2 == 0)
                        {
                            blnReturn = false;
                            //throw new DivideByZeroException("Divide a number by 0 is not allowed");
                            strError = "Divide a number by 0 is not allowed";
                        }
                        else
                        {
                            dblResult = dblNum1 / dblNum2;
                        }
                        break;
                }
            }
            catch(Exception ex)
            {
                blnReturn = false;
                //throw ex;
                strError= ex.Message;
            }
            return blnReturn;
        }
        #endregion
    }
}
