using ControleDeCinema.Testes.Interface.Compartilhado;

namespace ControleDeCinema.Testes.Interface.ModuloGeneroFilme;

[TestClass]
[TestCategory("Testes de Interface de G�nero de Filme")]
public sealed class GeneroFilmeInterfaceTestes : TestFixture
{
    [TestInitialize]
    public override void InicializarTeste()
    {
        base.InicializarTeste();

        RegistrarContaEmpresarial();
    }

    [TestMethod]
    public void Deve_Cadastrar_GeneroFilme_Corretamente()
    {
        // Arrange
        GeneroFilmeIndexPageObject generoFilmeIndex = new(driver);

        generoFilmeIndex
            .IrPara(enderecoBase);

        // Act
        GeneroFilmeFormPageObject generoFilmeForm = generoFilmeIndex
            .ClickCadastrar();

        generoFilmeForm
            .PreencherDescricao("Com�dia")
            .ClickSubmit();

        // Assert
        Assert.IsTrue(generoFilmeIndex.ContemGenero("Com�dia"));
    }

    [TestMethod]
    public void Deve_Editar_GeneroFilme_Corretamente()
    {
        // Arrange
        GeneroFilmeIndexPageObject generoFilmeIndex = new(driver);

        generoFilmeIndex
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherDescricao("Com�dia")
            .ClickSubmit();

        // Act
        GeneroFilmeFormPageObject generoFilmeForm = generoFilmeIndex
            .IrPara(enderecoBase)
            .ClickEditar();

        generoFilmeForm
            .PreencherDescricao("Rom�nce")
            .ClickSubmit();

        // Assert
        Assert.IsTrue(generoFilmeIndex.ContemGenero("Rom�nce"));
    }

    [TestMethod]
    public void Deve_Excluir_GeneroFilme_Corretamente()
    {
        // Arrange
        GeneroFilmeIndexPageObject generoFilmeIndex = new(driver);

        generoFilmeIndex
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherDescricao("Com�dia")
            .ClickSubmit();

        // Act
        GeneroFilmeFormPageObject generoFilmeForm = generoFilmeIndex
            .IrPara(enderecoBase)
            .ClickExcluir();

        generoFilmeForm
            .ClickSubmitExcluir("Rom�nce");

        // Assert
        Assert.IsFalse(generoFilmeIndex.ContemGenero("Rom�nce"));
    }

    [TestMethod]
    public void Deve_Visualizar_GenerosFilme_Cadastrados_Corretamente()
    {
        // Arrange
        GeneroFilmeIndexPageObject generoFilmeIndex = new(driver);

        generoFilmeIndex
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherDescricao("Com�dia")
            .ClickSubmit();

        generoFilmeIndex
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherDescricao("Rom�nce")
            .ClickSubmit();

        // Act
        generoFilmeIndex
            .IrPara(enderecoBase);

        // Assert
        Assert.IsTrue(generoFilmeIndex.ContemGenero("Com�dia"));
        Assert.IsTrue(generoFilmeIndex.ContemGenero("Rom�nce"));
    }

    [TestMethod]
    public void Nao_Deve_Cadastrar_GeneroFilme_Com_Campos_Vazios()
    {
        // Arrange
        GeneroFilmeIndexPageObject generoFilmeIndex = new(driver);

        generoFilmeIndex
            .IrPara(enderecoBase);

        // Act
        GeneroFilmeFormPageObject generoFilmeForm = generoFilmeIndex
            .ClickCadastrar();

        generoFilmeForm
            .ClickSubmitEsperandoErros();

        // Assert
        Assert.IsTrue(generoFilmeForm.EstourouValidacao("Descricao"));
    }

    [TestMethod]
    public void Nao_Deve_Cadastrar_GeneroFilme_Com_Descricao_Duplicada()
    {
        // Arrange
        GeneroFilmeIndexPageObject generoFilmeIndex = new(driver);

        generoFilmeIndex
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherDescricao("Com�dia")
            .ClickSubmit();

        // Act
        GeneroFilmeFormPageObject generoFilmeForm = generoFilmeIndex
            .IrPara(enderecoBase)
            .ClickCadastrar();

        generoFilmeForm
            .PreencherDescricao("Com�dia")
            .ClickSubmitEsperandoErros();

        // Assert
        Assert.IsTrue(generoFilmeForm.EstourouValidacao());
    }
}
