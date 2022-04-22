grammar MyGrammar;

/** The start rule; begin parsing here. */
prog: line*;

line
    : statement ';'
    | write ';'
    | read ';'
    | ';'
    | COMMENT
    ;

statement
    : declaration
    | assignment 
    | expr
    ;

    declaration
    : DATATYPE IDENTIFIER (',' IDENTIFIER)*
    ;

assignment
    : expr
    | IDENTIFIER '=' assignment
    ;

write
    : 'write' (formatedWrite)+
    ;

formatedWrite
    : STRING(',' expr ',')*(',' expr)*
    ;

read
    : 'read' IDENTIFIER (',' IDENTIFIER)*
    ;

expr
    : INT                           # int
    | FLOAT                         # float
    | BOOL                          # bool
    | STRING                        # string
    | IDENTIFIER                    # identifier
    | expr op=('*'|'/'|'%') expr    # mul
    | expr op=('+'|'-') expr        # add
    | STRING ('.') STRING           # concat
    | '(' expr ')'                  # par
    ;

DATATYPE : 'int' | 'string' | 'float' | 'bool' ;
IDENTIFIER : [a-zA-Z]+ ;

INT : ('-')?[1-9][0-9]* ;
FLOAT : ('-')?[0-9]+ '.' [0-9]+ ;
BOOL : 'true' | 'false' ;
STRING : ('"' ~'"'* '"') ;

WS : [ \t\r\n]+ -> skip ;
COMMENT : '//' ~( '\r' | '\n' )* -> skip;