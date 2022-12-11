grammar Lab1Calculator; 


/*
 * Parser Rules
 */

compileUnit : expression EOF;

expression :
	LPAREN expression RPAREN #ParenthesizedExpr
	|expression EXPONENT expression #ExponentialExpr
    | expression operatorToken=(MULTIPLY | DIVIDE) expression #MultiplicativeExpr
	| operatorToken=(MOD | DIV) LPAREN expression ';' expression RPAREN #ModDivExpr
	| expression operatorToken=(ADD | SUBTRACT) expression #AdditiveExpr
	| operatorToken=(INC|DEC) LPAREN expression RPAREN #IncDecExpr
	| operatorToken=(MIN|MAX) LPAREN expression ';' expression RPAREN #MaxMinExpr
	| NUMBER #NumberExpr
	| IDENTIFIER #IdentifierExpr
	; 
/*
 * Lexer Rules
 */

NUMBER : INT (',' INT)?; 
IDENTIFIER : [a-zA-Z]+[0-9]+;
INT : ('0'..'9')+;

MIN: 'min';
MAX: 'max';
EXPONENT : '^';
MULTIPLY : '*';
MOD: 'mod';
DIV: 'div';
INC: 'inc';
DEC: 'dec';
DIVIDE : '/';
SUBTRACT : '-';
ADD : '+';
LPAREN : '(';
RPAREN : ')';

WS : [ \t\r\n] -> channel(HIDDEN);