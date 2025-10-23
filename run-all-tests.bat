@echo off
echo ===============================================
echo    医疗影像管理系统 - 单元测试运行器
echo ===============================================
echo.

:: 设置变量
set SOLUTION_PATH=%~dp0MedicalImagingSystem.Tests.sln
set RESULTS_DIR=%~dp0TestResults
set COVERAGE_DIR=%~dp0Coverage

:: 创建结果目录
if not exist "%RESULTS_DIR%" mkdir "%RESULTS_DIR%"
if not exist "%COVERAGE_DIR%" mkdir "%COVERAGE_DIR%"

echo 正在运行所有测试项目的单元测试...
echo.

:: 运行所有测试并生成覆盖率报告
echo [1/5] 运行 Models 项目测试...
dotnet test "MedicalImagingSystem.Models.Tests\MedicalImagingSystem.Models.Tests.csproj" ^
    --logger "trx;LogFileName=ModelsTests.trx" ^
    --results-directory "%RESULTS_DIR%" ^
    --collect:"XPlat Code Coverage" ^
    --settings:coverlet.runsettings

echo [2/5] 运行 Services 项目测试...
dotnet test "MedicalImagingSystem.Services.Tests\MedicalImagingSystem.Services.Tests.csproj" ^
    --logger "trx;LogFileName=ServicesTests.trx" ^
    --results-directory "%RESULTS_DIR%" ^
    --collect:"XPlat Code Coverage" ^
    --settings:coverlet.runsettings

echo [3/5] 运行 ViewModels 项目测试...
dotnet test "MedicalImagingSystem.ViewModels.Tests\MedicalImagingSystem.ViewModels.Tests.csproj" ^
    --logger "trx;LogFileName=ViewModelsTests.trx" ^
    --results-directory "%RESULTS_DIR%" ^
    --collect:"XPlat Code Coverage" ^
    --settings:coverlet.runsettings

echo [4/5] 运行 Infrastructure 项目测试...
dotnet test "MedicalImagingSystem.Infrastructure.Tests\MedicalImagingSystem.Infrastructure.Tests.csproj" ^
    --logger "trx;LogFileName=InfrastructureTests.trx" ^
    --results-directory "%RESULTS_DIR%" ^
    --collect:"XPlat Code Coverage" ^
    --settings:coverlet.runsettings

echo [5/5] 运行 Logger 项目测试...
dotnet test "MedicalImagingSystem.Logger.Tests\MedicalImagingSystem.Logger.Tests.csproj" ^
    --logger "trx;LogFileName=LoggerTests.trx" ^
    --results-directory "%RESULTS_DIR%" ^
    --collect:"XPlat Code Coverage" ^
    --settings:coverlet.runsettings

echo.
echo ===============================================
echo 测试运行完成！
echo.
echo 测试结果文件位置: %RESULTS_DIR%
echo 代码覆盖率报告位置: %COVERAGE_DIR%
echo.
echo 要查看详细的测试结果，请检查 .trx 文件
echo 要生成 HTML 覆盖率报告，请运行: generate-coverage-report.bat
echo ===============================================

pause