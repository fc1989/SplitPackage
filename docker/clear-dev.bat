set srciptPath=%~dp0
if "%srciptPath:~-1%" equ "\" (
  set srciptPath=%srciptPath:~0,-1%
)
call docker-compose -f "%srciptPath%\docker-compose.yml" down --rmi local
call docker-compose -p splitpackage -f "%srciptPath%\splitpackage.yml" down --rmi local
docker volume prune
pause