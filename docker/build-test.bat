set srciptPath=%~dp0
if "%srciptPath:~-1%" equ "\" (
  set srciptPath=%srciptPath:~0,-1%
)
call docker-compose -f "%srciptPath%\docker-compose.yml" -f "%srciptPath%\docker-compose-test.yml" build
call docker-compose -f "%srciptPath%\docker-compose.yml" -f "%srciptPath%\docker-compose-test.yml" push
pause