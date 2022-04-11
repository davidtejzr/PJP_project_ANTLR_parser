grammar MyGrammar;

/** The start rule; begin parsing here. */
prog: statement + EOF;

statement
    : (expr ';')
    | DATATYPE IDENTIFIER ((',' IDENTIFIER)*)? ('=' expr)? ';'
    | IDENTIFIER (('='IDENTIFIER)*)?'=' expr ';' 
    | 'read' IDENTIFIER (',' IDENTIFIER)* ';'
    | 'write' expr (',' expr )* ';'
    | ';'
    ;

expr
    : IDENTIFIER                # identifier
    | expr op=('*'|'/') expr    # mul
    | expr op=('+'|'-') expr    # add
    | INT                       # int
    | FLOAT                     # float
    | OCT                       # oct
    | HEXA                      # hexa
    | STRING                    # string
    | '(' expr ')'              # par
    ;


DATATYPE : 'int' | 'string' | 'float' | 'bool' ;
ID : [a-zA-Z]+ ;
INT : [1-9][0-9]* ;
FLOAT : [0-9]+ '.' [0-9]+ ;
OCT : '0'[0-7]* ;
HEXA : '0x'[0-9a-fA-F]+ ;
STRING : ('"' ~'"'* '"') | ( '\'' ~'\''* '\'') ;

WS : [ \t\r\n]+ -> skip ;
IDENTIFIER : [a-zA-Z_][a-zA-Z0-9_]* ;