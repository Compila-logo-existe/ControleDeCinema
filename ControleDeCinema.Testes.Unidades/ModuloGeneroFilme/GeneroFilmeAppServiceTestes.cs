using ControledeCinema.Dominio.Compartilhado;
using ControleDeCinema.Aplicacao.ModuloGeneroFilme;
using ControleDeCinema.Dominio.ModuloAutenticacao;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using FluentResults;
using Microsoft.Extensions.Logging;
using Moq;

namespace ControleDeCinema.Testes.Unidades.ModuloGeneroFilme;

[TestClass]
[TestCategory("Testes de Unidade de GeneroFilmeAppService")]
public class GeneroFilmeAppServiceTestes
{
    // SUT
    private GeneroFilmeAppService generoFilmeAppService;

    // MOCK
    private Mock<ITenantProvider> tenantProviderMock;
    private Mock<IRepositorioGeneroFilme> repositorioGeneroFilmeMock;
    private Mock<IUnitOfWork> unitOfWorkMock;
    private Mock<ILogger<GeneroFilmeAppService>> loggerMock;

    [TestInitialize]
    public void Setup()
    {
        tenantProviderMock = new Mock<ITenantProvider>();
        repositorioGeneroFilmeMock = new Mock<IRepositorioGeneroFilme>();
        unitOfWorkMock = new Mock<IUnitOfWork>();
        loggerMock = new Mock<ILogger<GeneroFilmeAppService>>();

        generoFilmeAppService = new GeneroFilmeAppService(
            tenantProviderMock.Object,
            repositorioGeneroFilmeMock.Object,
            unitOfWorkMock.Object,
            loggerMock.Object
        );
    }

    #region Testes Cadastro
    [TestMethod]
    public void Cadastrar_GeneroFilme_Deve_Retornar_Sucesso()
    {
        // Arrange
        GeneroFilme novoGenero = new("Com�dia");

        repositorioGeneroFilmeMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<GeneroFilme>());

        // Act
        Result resultadoCadastro = generoFilmeAppService.Cadastrar(novoGenero);

        // Assert
        repositorioGeneroFilmeMock.Verify(r => r.Cadastrar(novoGenero), Times.Once);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Once);

        Assert.IsNotNull(resultadoCadastro);
        Assert.IsTrue(resultadoCadastro.IsSuccess);
    }

    [TestMethod]
    public void Cadastrar_GeneroFilme_Duplicada_Deve_Retornar_Falha()
    {
        // Arrange
        repositorioGeneroFilmeMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<GeneroFilme>() { new("Com�dia") });

        GeneroFilme novoGenero = new("Com�dia");

        // Act
        Result resultadoCadastro = generoFilmeAppService.Cadastrar(novoGenero);

        // Assert
        repositorioGeneroFilmeMock.Verify(r => r.Cadastrar(novoGenero), Times.Never);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Never);

        string mensagemErro = resultadoCadastro.Errors[0].Message;

        Assert.IsNotNull(resultadoCadastro);
        Assert.IsTrue(resultadoCadastro.IsFailed);
        Assert.AreEqual("Registro duplicado", mensagemErro);
    }

    [TestMethod]
    public void Cadastrar_GeneroFilme_Com_Excecao_Lancada_Deve_Retornar_Falha()
    {
        // Arrange
        GeneroFilme novoGenero = new("Com�dia");

        repositorioGeneroFilmeMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<GeneroFilme>());

        repositorioGeneroFilmeMock
            .Setup(r => r.Cadastrar(novoGenero))
            .Throws(new Exception("Erro inesperado"));

        unitOfWorkMock
            .Setup(u => u.Commit())
            .Throws(new Exception("Erro no cadastro"));

        // Act
        Result resultadoCadastro = generoFilmeAppService.Cadastrar(novoGenero);

        // Assert
        unitOfWorkMock.Verify(u => u.Rollback(), Times.Once);

        string mensagemErro = resultadoCadastro.Errors[0].Message;

        Assert.IsNotNull(resultadoCadastro);
        Assert.IsTrue(resultadoCadastro.IsFailed);
        Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
    }
    #endregion

    #region Testes Edi��o
    [TestMethod]
    public void Editar_GeneroFilme_Deve_Retornar_Sucesso()
    {
        // Arrange
        GeneroFilme novoGenero = new("Com�dia");

        repositorioGeneroFilmeMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<GeneroFilme>() { novoGenero });

        repositorioGeneroFilmeMock
            .Setup(r => r.SelecionarRegistroPorId(novoGenero.Id))
            .Returns(novoGenero);

        GeneroFilme generoEditado = new("Com�dia Super Ingrasada");

        // Act
        Result resultadoEdicao = generoFilmeAppService.Editar(novoGenero.Id, generoEditado);

        // Assert
        repositorioGeneroFilmeMock.Verify(r => r.Editar(novoGenero.Id, generoEditado), Times.Once);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Once);

        Assert.IsNotNull(resultadoEdicao);
        Assert.IsTrue(resultadoEdicao.IsSuccess);
    }

    [TestMethod]
    public void Editar_GeneroFilme_Duplicada_Deve_Retornar_Falha()
    {
        // Arrange
        GeneroFilme novoGenero = new("Com�dia");

        List<GeneroFilme> generosFilmeExistentes = new()
        {
            novoGenero,
            new("Com�dia Super Ingrasada")
        };

        repositorioGeneroFilmeMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(generosFilmeExistentes);

        GeneroFilme generoEditado = new("Com�dia Super Ingrasada");

        // Act
        Result resultadoEdicao = generoFilmeAppService.Editar(novoGenero.Id, generoEditado);

        // Assert
        repositorioGeneroFilmeMock.Verify(r => r.Editar(novoGenero.Id, generoEditado), Times.Never);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Never);

        string mensagemErro = resultadoEdicao.Errors[0].Message;

        Assert.IsNotNull(resultadoEdicao);
        Assert.IsTrue(resultadoEdicao.IsFailed);
        Assert.AreEqual("Registro duplicado", mensagemErro);
    }

    [TestMethod]
    public void Editar_GeneroFilme_Com_Excecao_Lancada_Deve_Retornar_Falha()
    {
        // Arrange
        GeneroFilme novoGenero = new("Com�dia");

        repositorioGeneroFilmeMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<GeneroFilme> { novoGenero });

        GeneroFilme generoEditado = new("Com�dia Super Ingrasada");

        repositorioGeneroFilmeMock
            .Setup(r => r.Editar(novoGenero.Id, generoEditado))
            .Throws(new Exception("Erro inesperado"));

        unitOfWorkMock
            .Setup(u => u.Commit())
            .Throws(new Exception("Erro na edi��o"));

        // Act
        Result resultadoEdicao = generoFilmeAppService.Editar(novoGenero.Id, generoEditado);

        // Assert
        unitOfWorkMock.Verify(u => u.Rollback(), Times.Once);

        string mensagemErro = resultadoEdicao.Errors[0].Message;

        Assert.IsNotNull(resultadoEdicao);
        Assert.IsTrue(resultadoEdicao.IsFailed);
        Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
    }
    #endregion

    #region Testes Exclus�o
    [TestMethod]
    public void Excluir_GeneroFilme_Deve_Retornar_Sucesso()
    {
        // Arrange
        GeneroFilme novoGenero = new("Com�dia");

        repositorioGeneroFilmeMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<GeneroFilme> { novoGenero });

        repositorioGeneroFilmeMock
            .Setup(r => r.SelecionarRegistroPorId(novoGenero.Id))
            .Returns(novoGenero);

        // Act
        Result resultadoExclusao = generoFilmeAppService.Excluir(novoGenero.Id);

        // Assert
        repositorioGeneroFilmeMock.Verify(r => r.Excluir(novoGenero.Id), Times.Once);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Once);

        Assert.IsNotNull(resultadoExclusao);
        Assert.IsTrue(resultadoExclusao.IsSuccess);
    }

    [TestMethod]
    public void Excluir_GeneroFilme_Com_Excecao_Lancada_Deve_Retornar_Falha()
    {
        // Arrange
        GeneroFilme novoGenero = new("Com�dia");

        repositorioGeneroFilmeMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<GeneroFilme> { novoGenero });

        repositorioGeneroFilmeMock
            .Setup(r => r.Excluir(novoGenero.Id))
            .Throws(new Exception("Erro inesperado"));

        unitOfWorkMock
            .Setup(r => r.Commit())
            .Throws(new Exception("Erro na exclus�o"));

        // Act
        Result resultadoExclusao = generoFilmeAppService.Excluir(novoGenero.Id);

        // Assert
        unitOfWorkMock.Verify(u => u.Rollback(), Times.Once);

        string mensagemErro = resultadoExclusao.Errors[0].Message;

        Assert.IsNotNull(resultadoExclusao);
        Assert.IsTrue(resultadoExclusao.IsFailed);
        Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
    }
    #endregion

    #region Testes Sele��o por Id
    [TestMethod]
    public void Selecionar_GeneroFilme_Por_Id_Deve_Retornar_Sucesso()
    {
        // Arrange
        GeneroFilme novoGenero = new("Com�dia");

        repositorioGeneroFilmeMock
            .Setup(r => r.SelecionarRegistroPorId(novoGenero.Id))
            .Returns(novoGenero);

        // Act
        Result<GeneroFilme> resultadoSelecao = generoFilmeAppService.SelecionarPorId(novoGenero.Id);

        GeneroFilme generoSelecionado = resultadoSelecao.ValueOrDefault;

        // Assert
        repositorioGeneroFilmeMock.Verify(r => r.SelecionarRegistroPorId(novoGenero.Id), Times.Once);

        Assert.IsNotNull(resultadoSelecao);
        Assert.IsTrue(resultadoSelecao.IsSuccess);
        Assert.IsNotNull(generoSelecionado);
        Assert.AreEqual(novoGenero, generoSelecionado);
    }

    [TestMethod]
    public void Selecionar_GeneroFilme_Por_Id_Id_Inexistente_Deve_Retornar_Falha()
    {
        // Arrange
        GeneroFilme novoGenero = new("Com�dia");

        repositorioGeneroFilmeMock
            .Setup(r => r.SelecionarRegistroPorId(novoGenero.Id))
            .Returns(novoGenero);

        // Act
        Result<GeneroFilme> resultadoSelecao = generoFilmeAppService.SelecionarPorId(Guid.NewGuid());

        GeneroFilme generoSelecionado = resultadoSelecao.ValueOrDefault;

        // Assert
        repositorioGeneroFilmeMock.Verify(r => r.SelecionarRegistroPorId(novoGenero.Id), Times.Never);

        string mensagemErro = resultadoSelecao.Errors[0].Message;

        Assert.IsNotNull(resultadoSelecao);
        Assert.IsTrue(resultadoSelecao.IsFailed);
        Assert.IsNull(generoSelecionado);
        Assert.AreNotEqual(novoGenero, generoSelecionado);
        Assert.AreEqual("Registro n�o encontrado", mensagemErro);
    }

    [TestMethod]
    public void Selecionar_GeneroFilme_Por_Id_Com_Excecao_Lancada_Deve_Retornar_Falha()
    {
        // Arrange
        GeneroFilme novoGenero = new("Com�dia");

        repositorioGeneroFilmeMock
            .Setup(r => r.SelecionarRegistroPorId(novoGenero.Id))
            .Throws(new Exception("Erro inesperado"));

        // Act
        Result<GeneroFilme> resultadoSelecao = generoFilmeAppService.SelecionarPorId(novoGenero.Id);

        GeneroFilme generoSelecionado = resultadoSelecao.ValueOrDefault;

        // Assert
        repositorioGeneroFilmeMock.Verify(r => r.SelecionarRegistroPorId(novoGenero.Id), Times.Once);

        string mensagemErro = resultadoSelecao.Errors[0].Message;

        Assert.IsNotNull(resultadoSelecao);
        Assert.IsTrue(resultadoSelecao.IsFailed);
        Assert.IsNull(generoSelecionado);
        Assert.AreNotEqual(novoGenero, generoSelecionado);
        Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
    }
    #endregion

    #region Testes Sele��o de Todos
    [TestMethod]
    public void Selecionar_Todos_GenerosFilme_Deve_Retornar_Sucesso()
    {
        // Arrange
        GeneroFilme novoGenero = new("Com�dia");

        List<GeneroFilme> generosFilmeExistentes = new()
        {
            novoGenero,
            new("Rom�nce")
        };

        repositorioGeneroFilmeMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(generosFilmeExistentes);

        // Act
        Result<List<GeneroFilme>> resultadosSelecao = generoFilmeAppService.SelecionarTodos();

        List<GeneroFilme> generosSelecionados = resultadosSelecao.Value;

        // Assert
        repositorioGeneroFilmeMock.Verify(r => r.SelecionarRegistros(), Times.Once);

        Assert.IsNotNull(resultadosSelecao);
        Assert.IsTrue(resultadosSelecao.IsSuccess);
        Assert.IsNotNull(generosSelecionados);
        CollectionAssert.AreEquivalent(generosFilmeExistentes, generosSelecionados);
    }

    [TestMethod]
    public void Selecionar_Todos_GenerosFilme_Com_Excecao_Lancada_Deve_Retornar_Falha()
    {
        // Arrange
        GeneroFilme novoGenero = new("Com�dia");

        List<GeneroFilme> generosFilmeExistentes = new()
        {
            novoGenero,
            new("Rom�nce")
        };

        repositorioGeneroFilmeMock
            .Setup(r => r.SelecionarRegistros())
            .Throws(new Exception("Erro inesperado"));

        // Act
        Result<List<GeneroFilme>> resultadosSelecao = generoFilmeAppService.SelecionarTodos();

        List<GeneroFilme> generosSelecionados = resultadosSelecao.ValueOrDefault;

        // Assert
        repositorioGeneroFilmeMock.Verify(r => r.SelecionarRegistros(), Times.Once);

        string mensagemErro = resultadosSelecao.Errors[0].Message;

        Assert.IsNotNull(resultadosSelecao);
        Assert.IsTrue(resultadosSelecao.IsFailed);
        Assert.IsNull(generosSelecionados);
        CollectionAssert.AreNotEquivalent(generosFilmeExistentes, generosSelecionados);
        Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
    }
    #endregion
}
