#include "calculator.h"
#include <QtWidgets>
#include <cmath>
#include "button.h"


Button *Calculator::createButton(const QString &text, const char *slot)
{
    Button *button = new Button(text);
    connect(button, SIGNAL(clicked()), this, slot);
    return button;
}

Calculator::Calculator(QWidget *parent)
    : QWidget(parent)
{
    isAdd=false;
    isSubstract=false;
    isMultiply=false;
    isDivide=false;

    Result = 0.0;
    setWindowTitle(tr("CppCalculator"));
    this->resize(350, 300);
    QGridLayout *mainLayout = new QGridLayout;
    setLayout(mainLayout);
    //mainLayout->setSizeConstraint(QLayout::SetFixedSize);

    display = new QLineEdit("0");
    display->setReadOnly(true);
    display->setAlignment(Qt::AlignRight);
    display->setMaxLength(15);

    for (int i = 0; i < NumDigitButtons; ++i) {
        digitButtons[i] = createButton(QString::number(i), SLOT(digitClicked()));
    }

    for (int i = 1; i < NumDigitButtons; ++i) {
        int row = ((9 - i) / 3) + 2;
        int column = ((i - 1) % 3) + 1;
        mainLayout->addWidget(digitButtons[i], row, column);
    }

    Button* divisionButton = createButton(tr("\303\267"), SLOT(divisionClicked()));
    Button* multButton = createButton(tr("\303\227"), SLOT(multClicked()));
    Button* minusButton = createButton(tr("-"), SLOT(minusClicked()));
    Button* plusButton = createButton(tr("+"), SLOT(plusClicked()));
    Button* pointButton = createButton(tr("."), SLOT(pointClicked()));
    Button* equalButton = createButton(tr("="), SLOT(equalClicked()));
    Button* sqrtButton = createButton(tr("sqrt"), SLOT(sqrtClicked()));
    Button* powButton = createButton(tr("pow"), SLOT(powClicked()));
    Button* clearButton = createButton(tr("CE"), SLOT(clearClicked()));
    Button* signButton = createButton(tr("\302\261"), SLOT(signClicked()));
    Button* backspaceButton = createButton(tr("←"), SLOT(backspaceClicked()));
    Button* percentButton = createButton(tr("%"), SLOT(percentClicked()));
    Button* addToMemoryButton = createButton(tr("M+"), SLOT(addToMemoryClicked()));
    Button* clearMemoryButton = createButton(tr("M-"), SLOT(clearMemoryClicked()));

    mainLayout->addWidget(display,0,0,1,6);        //lineEdit
    mainLayout->addWidget(digitButtons[0], 5, 1);  //"0"
    mainLayout->addWidget(pointButton, 5, 2);      //"."
    mainLayout->addWidget(equalButton, 5, 3);      //"="

    mainLayout->addWidget(divisionButton, 2, 4);   //"/"
    mainLayout->addWidget(multButton, 3, 4);       //"*"
    mainLayout->addWidget(minusButton, 4, 4);      //"-"
    mainLayout->addWidget(plusButton, 5, 4);       //"+"

    mainLayout->addWidget(powButton, 2, 0);        //"pow"
    mainLayout->addWidget(sqrtButton, 3, 0);        //"sqrt"
    mainLayout->addWidget(signButton, 4, 0);        //"sign"
    mainLayout->addWidget(clearButton, 5, 0);        //"CLEAR"

    mainLayout->addWidget(percentButton, 1, 1);        //"%"
    mainLayout->addWidget(addToMemoryButton, 1, 2);        //"M-"
    mainLayout->addWidget(clearMemoryButton, 1, 3);        //"M+"
    mainLayout->addWidget(backspaceButton, 1, 4);        //"<-"

    QPixmap pixCalc("C:/Users/ZpmPower/Documents/CppCalculator/Images/buzka.png");
    QLabel* calc = new QLabel();
    calc->setPixmap(pixCalc);
    mainLayout->addWidget(calc, 0, 0);

}
void Calculator::digitClicked()
{
    Button *clickedButton = qobject_cast<Button *>(sender());
    int digitValue = clickedButton->text().toInt();
    if (display->text() == "0" && digitValue == 0.0) return;

    if (iswaitingOperand) {
        display->clear();
        iswaitingOperand = false;
    }
    display->setText(display->text() + QString::number(digitValue));
}

void Calculator::signClicked()
{
    Button *clickedButton = qobject_cast<Button *>(sender());
    double symbol;
    if(clickedButton->text() == tr("\302\261"))
    {
        symbol = display->text().toDouble();
        symbol = symbol * -1;
        display->setText(QString::number(symbol));
    }
}

void Calculator::percentClicked()
{
    Button *clickedButton = qobject_cast<Button *>(sender());
    double symbol;
    if(clickedButton->text() == tr("%"))
    {
        symbol = display->text().toDouble();
        symbol = symbol * 0.01;
        display->setText(QString::number(symbol));
    }
}


void Calculator::plusClicked()
{
    firstValue=display->text().toDouble();
    display->setText("");
    isAdd=true;
}

void Calculator::minusClicked()
{
    firstValue=display->text().toDouble();
    display->setText("");
    isSubstract=true;
}

void Calculator::multClicked()
{
    firstValue=display->text().toDouble();
    display->setText("");
    isMultiply=true;
}

void Calculator::divisionClicked()
{
    firstValue=display->text().toDouble();
    display->setText("");
    isDivide=true;
}

void Calculator::powClicked()
{
    firstValue=display->text().toDouble();
    display->setText("");
    isPow=true;
}

void Calculator::sqrtClicked()
{
    firstValue=display->text().toDouble();
    isSqrt=true;
}

void Calculator::pointClicked()
{
    if (iswaitingOperand)
        display->setText("0");
    if (!display->text().contains('.'))
        display->setText(display->text() + tr("."));
    iswaitingOperand= false;
}
void Calculator::equalClicked()
{
    secondValue=display->text().toDouble();
    if(isAdd){
        display->setText(QString::number(firstValue+secondValue));
    }
    else if(isSubstract)
    {
        display->setText(QString::number(firstValue-secondValue));
    }
    else if(isMultiply)
    {
        display->setText(QString::number(firstValue*secondValue));
    }
    else if(isDivide)
    {
        display->setText(QString::number(firstValue/secondValue));
    }
    else if(isPow)
    {
        display->setText(QString::number(pow(firstValue,secondValue)));
    }
    else if(isSqrt)
    {
        display->setText(QString::number(sqrt(firstValue)));
    }
}

void Calculator::backspaceClicked()
{
    if (iswaitingOperand)
        return;

    QString text = display->text();
    text.chop(1);
    if (text.isEmpty()) {
        text = "0";
        iswaitingOperand = true;
    }
    display->setText(text);
}

void Calculator::clearClicked()
{
    firstValue = 0.0;
    secondValue = 0.0;
    display->setText("0");
    iswaitingOperand = true;
}

void Calculator::clearMemoryClicked()
{
    firstValue = 0.0;
    secondValue = 0.0;
}

void Calculator::addToMemoryClicked()
{   double result;
    equalClicked();
    result=firstValue+secondValue;
}
