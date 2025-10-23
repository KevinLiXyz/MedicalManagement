# 医疗影像管理系统 - 单元测试文档

## 概述

本文档描述了医疗影像管理系统的完整单元测试架构。我们为系统的每个核心项目都创建了对应的测试项目，确保代码质量和可靠性。

## 测试项目结构

```
MedicalManagement/
├── MedicalImagingSystem.Models.Tests/           # Models层单元测试
│   ├── PatientModelTests.cs                     # 患者模型测试
│   ├── MedicalImageModelTests.cs                # 医疗图像模型测试
│   └── MedicalImagingSystem.Models.Tests.csproj
│
├── MedicalImagingSystem.Services.Tests/         # Services层单元测试
│   ├── PatientServiceTests.cs                   # 患者服务测试
│   ├── ImageServiceTests.cs                     # 图像服务测试
│   └── MedicalImagingSystem.Services.Tests.csproj
│
├── MedicalImagingSystem.ViewModels.Tests/       # ViewModels层单元测试
│   ├── ImageViewerViewModelTests.cs             # 图像查看器ViewModel测试
│   └── MedicalImagingSystem.ViewModels.Tests.csproj
│
├── MedicalImagingSystem.Infrastructure.Tests/   # Infrastructure层单元测试
│   ├── DependencyInjectionConfigTests.cs        # 依赖注入配置测试
│   └── MedicalImagingSystem.Infrastructure.Tests.csproj
│
├── MedicalImagingSystem.Logger.Tests/           # Logger层单元测试
│   ├── LogManagerTests.cs                       # 日志管理器测试
│   └── MedicalImagingSystem.Logger.Tests.csproj
│
├── run-all-tests.bat                            # Windows测试运行脚本
├── run-all-tests.sh                             # Linux/Mac测试运行脚本
├── coverlet.runsettings                         # 覆盖率配置
└── MedicalImagingSystem.Tests.sln               # 测试解决方案文件
```

## 测试技术栈

- **测试框架**: xUnit.net 2.9.2
- **断言库**: FluentAssertions 6.10.0
- **模拟框架**: Moq 4.20.69
- **覆盖率工具**: Coverlet
- **.NET版本**: .NET 8.0

## 测试覆盖范围

### 1. Models项目测试 (MedicalImagingSystem.Models.Tests)

#### PatientModelTests
- ✅ 构造函数初始化
- ✅ 属性设置和获取
- ✅ 年龄计算逻辑
- ✅ PropertyChanged事件触发
- ✅ 图像集合管理
- ✅ 空值和边界条件处理

#### MedicalImageModelTests
- ✅ 模型属性验证
- ✅ 文件大小格式化
- ✅ 医疗图像元数据管理
- ✅ PropertyChanged事件
- ✅ 异常情况处理

### 2. Services项目测试 (MedicalImagingSystem.Services.Tests)

#### PatientServiceTests
- ✅ 患者数据CRUD操作
- ✅ 异步方法测试
- ✅ 搜索功能验证
- ✅ 数据验证逻辑
- ✅ 边界条件和异常处理
- ✅ 患者与图像关联

#### ImageServiceTests
- ✅ 图像加载功能
- ✅ 支持格式验证
- ✅ 图像元数据提取
- ✅ 缓存机制
- ✅ 并发处理
- ✅ 错误处理

### 3. ViewModels项目测试 (MedicalImagingSystem.ViewModels.Tests)

#### ImageViewerViewModelTests
- ✅ MVVM模式实现
- ✅ 命令处理 (ZoomIn, ZoomOut, Reset)
- ✅ 属性绑定和通知
- ✅ 图像加载状态管理
- ✅ 缩放和平移功能
- ✅ 鼠标交互逻辑
- ✅ 异步操作处理

### 4. Infrastructure项目测试 (MedicalImagingSystem.Infrastructure.Tests)

#### DependencyInjectionConfigTests
- ✅ 服务注册验证
- ✅ 生命周期管理
- ✅ 配置加载
- ✅ 日志服务配置
- ✅ 作用域管理
- ✅ 服务解析

### 5. Logger项目测试 (MedicalImagingSystem.Logger.Tests)

#### LogManagerTests
- ✅ 日志级别处理 (Info, Warning, Error)
- ✅ 异常日志记录
- ✅ 空值处理
- ✅ 接口实现验证
- ✅ Mock验证

## 如何运行测试

### Windows环境
```bash
# 运行所有测试
.\run-all-tests.bat

# 运行特定项目测试
dotnet test MedicalImagingSystem.Models.Tests\MedicalImagingSystem.Models.Tests.csproj

# 运行测试并生成覆盖率报告
dotnet test --collect:"XPlat Code Coverage" --settings:coverlet.runsettings
```

### Linux/Mac环境
```bash
# 赋予执行权限
chmod +x run-all-tests.sh

# 运行所有测试
./run-all-tests.sh

# 运行特定项目测试
dotnet test MedicalImagingSystem.Models.Tests/MedicalImagingSystem.Models.Tests.csproj
```

### Visual Studio
1. 打开 `MedicalImagingSystem.Tests.sln`
2. 在测试资源管理器中运行测试
3. 查看测试结果和覆盖率

## 测试结果输出

### 测试报告文件
- `TestResults/ModelsTests.trx` - Models项目测试结果
- `TestResults/ServicesTests.trx` - Services项目测试结果
- `TestResults/ViewModelsTests.trx` - ViewModels项目测试结果
- `TestResults/InfrastructureTests.trx` - Infrastructure项目测试结果
- `TestResults/LoggerTests.trx` - Logger项目测试结果

### 覆盖率报告
- XML格式覆盖率报告用于CI/CD集成
- 支持多种覆盖率格式 (OpenCover, Cobertura)

## 测试最佳实践

### 1. 测试命名规范
```csharp
[Fact]
public void MethodName_StateUnderTest_ExpectedBehavior()
{
    // Arrange - 准备测试数据
    // Act - 执行测试方法
    // Assert - 验证结果
}
```

### 2. 使用Theory进行参数化测试
```csharp
[Theory]
[InlineData("valid_input")]
[InlineData("another_input")]
public void Method_WithDifferentInputs_ShouldBehaveCorrectly(string input)
{
    // 测试逻辑
}
```

### 3. Mock和依赖注入
```csharp
private readonly Mock<ILogger<ServiceClass>> _mockLogger;
private readonly ServiceClass _service;

public ServiceTests()
{
    _mockLogger = new Mock<ILogger<ServiceClass>>();
    _service = new ServiceClass(_mockLogger.Object);
}
```

### 4. 异步测试
```csharp
[Fact]
public async Task AsyncMethod_WithValidInput_ShouldReturnExpectedResult()
{
    // Arrange
    var input = "test";

    // Act
    var result = await _service.ProcessAsync(input);

    // Assert
    result.Should().NotBeNull();
}
```

## 持续集成支持

测试配置支持以下CI/CD平台：
- ✅ GitHub Actions
- ✅ Azure DevOps
- ✅ Jenkins
- ✅ TeamCity

## 测试覆盖率目标

- **目标覆盖率**: ≥80%
- **关键业务逻辑**: ≥95%
- **Models层**: ≥90%
- **Services层**: ≥85%
- **ViewModels层**: ≥85%

## 故障排除

### 常见问题
1. **包恢复失败**: 运行 `dotnet restore`
2. **测试发现失败**: 检查项目引用和包版本
3. **覆盖率报告为空**: 确认coverlet.runsettings配置正确

### 依赖问题
如果遇到依赖冲突，可以尝试：
```bash
dotnet clean
dotnet restore --force
dotnet build
```

## 贡献指南

添加新测试时请遵循：
1. 每个新类都应有对应的测试类
2. 测试方法应覆盖所有公共方法
3. 包含正常流程和异常流程测试
4. 使用有意义的测试方法名
5. 添加适当的测试数据和边界条件测试

## 联系信息

如有测试相关问题，请联系开发团队或查看项目文档。

---
*最后更新时间: 2025年9月25日*