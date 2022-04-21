grammar MyGrammar;

/** The start rule; begin parsing here. */
prog: line*;

line
    : statement ';'
    /**| print ';'*/
    | read ';'
    | COMMENT
    | ';'
    ;

statement
    : declaration
    | assignment 
    | expr
    ;

read : 'read' ID (',' ID)* ;
print : 'write' (formated)+ ;
formated : STRING(',' expr ',')*(',' expr)* ;

declaration
    : DATATYPE IDENTIFIER (',' IDENTIFIER)*
    ;

assignment
    : expr
    | IDENTIFIER '=' assignment
    ;

expr
    : CONSTANT                      # constant
    | IDENTIFIER                    # identifier
    | expr op=('*'|'/'|'%') expr    # mul
    | expr op=('+'|'-') expr        # add
    | STRING ('.') STRING           # concat
    | '(' expr ')'                  # par
    ;

CONSTANT : INT | FLOAT | BOOL | STRING ;
DATATYPE : 'int' | 'string' | 'float' | 'bool' ;
IDENTIFIER : [a-zA-Z]+ ;

INT : ('-')?[1-9][0-9]* ;
FLOAT : ('-')?[0-9]+ '.' [0-9]+ ;
BOOL : 'true' | 'false' ;
STRING : ('"' ~'"'* '"') | ( '\'' ~'\''* '\'') ;


WS : [ \t\r\n]+ -> skip ;
COMMENT : '//' ~( '\r' | '\n' )* -> skip;