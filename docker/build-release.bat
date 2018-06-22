set srciptPath=%~dp0
if "%srciptPath:~-1%" equ "\" (
  set srciptPath=%srciptPath:~0,-1%
)
call docker-compose -f "%srciptPath%\docker-compose.yml" build
call docker-compose -p splitpackage -f "%srciptPath%\splitpackage.yml" up
pause