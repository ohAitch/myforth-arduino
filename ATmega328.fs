\ ATmega328.fs

\ ----- "Low" Special Function Registers ----- /
\ can be addressed with IN or OUT

$23 constant PINB
$24 constant DDRB
$25 constant PORTB
$26 constant PINC
$27 constant DDRC
$28 constant PORTC
$29 constant PIND
$2a constant DDRD
$2b constant PORTD

$35 constant TIFR0
$36 constant TIFR1
$37 constant TIFR2
$3b constant PCIFR
$3c constant EIFR
$3d constant EIMSK
$3e constant GPIOR0
$3f constant EECR

$40 constant EEDR
$41 constant EEARL
$42 constant EEARH
$43 constant GTCCR
$44 constant TCCR0A
$45 constant TCCR0B
$46 constant TCNT0
$47 constant OCR0A
$48 constant OCR0B
$4a constant GPIOR1
$4b constant GPIOR2
$4c constant SPCR
$4d constant SPSR
$4e constant SPDR

$50 constant ACSR
$53 constant SMCR
$54 constant MCUSR
$55 constant MCUCR
$57 constant SPMCSR
$5d constant SPL
$5e constant SPH
$5f constant SREG

\ ----- "High" Special Function Registers ----- /
\ can't be addressed with IN or OUT

$60 constant WDTCSR
$61 constant CLKPR
$64 constant PRR
$66 constant OSCCAL
$68 constant PCICR
$69 constant EICRA
$6b constant PCMSK0
$6c constant PCMSK1
$6d constant PCMSK2
$6e constant TIMSK0
$6f constant TIMSK1

$70 constant TIMSK2
$78 constant ADCL
$79 constant ADCH
$7a constant ADCSRA
$7b constant ADCSRB
$7c constant ADMUX
$7e constant DIDR0
$7f constant DIDR1

$80 constant TCCR1A
$81 constant TCCR1B
$82 constant TCCR1C
$84 constant TCNT1L
$85 constant TCNT1H
$86 constant ICR1L
$87 constant ICR1H
$88 constant OCR1AL
$89 constant OCR1AH
$8a constant OCR1BL
$8b constant OCR1BH

$b0 constant TCCR2A
$b1 constant TCCR2B
$b2 constant TCNT2
$b3 constant OCR2A
$b4 constant OCR2B
$b6 constant ASSR
$b8 constant TWBR
$b9 constant TWSR
$ba constant TWAR
$bb constant TWDR
$bc constant TWCR
$bd constant TWAMR

$c0 constant UCSR0A
$c1 constant UCSR0B
$c2 constant UCSR0C
$c4 constant UBRR0L  \ USART Baud Rate Register Low
$c5 constant UBRR0H  \ USART Baud Rate Register High
$c6 constant UDR0    \ USART I/O Data Register

: PORT ( n - bit port)   dup 8 < if PORTD exit then  -8 + PORTB ;
: DDR ( n - bit port)   dup 8 < if DDRD exit then  -8 + DDRB ;
: PIN ( n - bit port)   dup 8 < if PIND exit then  -8 + PINB ;
   
