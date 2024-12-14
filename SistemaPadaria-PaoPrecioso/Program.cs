using System;
using System.Collections.Generic;
using System.IO;

namespace PadariaPaoPrecioso
{
    struct Produto
    {
        public int Codigo;
        public string Descricao;
        public float Preco;
        public DateTime Validade;
        public int Quantidade;
    }

    class Program
    {
        static List<Produto> listaProdutos = new List<Produto>();
        static string caminhoArquivoBinario = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "produtos.bin");
        static string caminhoRegistroVendas = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "RegistroDeVendas.txt");

        static void Main(string[] args)
        {
            try
            {
                CarregarProdutos();
                ExibirMensagemInicial();
                ExibirTelaInicial();
                MenuPrincipal();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"ERRO CRÍTICO: {ex.Message}");
                Console.ResetColor();
            }
        }

        static void ExibirMensagemInicial()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("===============================================");
            Console.WriteLine("       CÓDIGO DESENVOLVIDO POR VITOR MANOEL    ");
            Console.WriteLine("===============================================");
            Console.ResetColor();
            System.Threading.Thread.Sleep(2000);
        }

        static void ExibirTelaInicial()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("================================================================");
            Console.WriteLine("  SISTEMA DA PADARIA PÃO PRECIOSO - PROPRIEDADE DO SR. ANTÔNIO  ");
            Console.WriteLine("================================================================");
            Console.ResetColor();
            System.Threading.Thread.Sleep(2000);
        }

        static void MenuPrincipal()
        {
            int opcao;
            do
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("===============================================");
                Console.WriteLine("           MENU PRINCIPAL - PADARIA            ");
                Console.WriteLine("===============================================");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("1. Cadastrar Produto");
                Console.WriteLine("2. Listar Produtos");
                Console.WriteLine("3. Registrar Venda");
                Console.WriteLine("4. Buscar Produto");
                Console.WriteLine("5. Verificar Estoque Baixo");
                Console.WriteLine("6. Verificar Produtos Vencidos");
                Console.WriteLine("7. Gerar Relatório de Produtos");
                Console.WriteLine("8. Consultar Registro de Vendas");
                Console.WriteLine("9. Repor Estoque");
                Console.WriteLine("10. Gerar Relatório de Fluxo de Caixa");
                Console.WriteLine("11. Sair");
                Console.WriteLine("===============================================");
                Console.ResetColor();

                Console.Write("Escolha uma opção: ");
                if (!int.TryParse(Console.ReadLine(), out opcao))
                {
                    ExibirMensagemErro("Entrada inválida! Tente novamente.");
                    continue;
                }

                switch (opcao)
                {
                    case 1:
                        CadastrarProduto();
                        break;
                    case 2:
                        ListarProdutos();
                        break;
                    case 3:
                        RegistrarVenda();
                        break;
                    case 4:
                        BuscarProduto();
                        break;
                    case 5:
                        VerificarEstoqueBaixoRecursivo(0);
                        break;
                    case 6:
                        VerificarProdutosVencidosRecursivo(0);
                        break;
                    case 7:
                        GerarRelatorioProdutos();
                        break;
                    case 8:
                        ConsultarRegistroDeVendas();
                        break;
                    case 9:
                        ReporEstoque();
                        break;
                    case 10:
                        GerarRelatorioFluxoDeCaixa();
                        break;
                    case 11:
                        ExibirMensagemSucesso("Encerrando o sistema... Até logo!");
                        break;
                    default:
                        ExibirMensagemErro("Opção inválida! Escolha novamente.");
                        break;
                }

                // Verificação automática de estoque baixo
                if (opcao != 11)
                {
                    VerificarEstoqueAutomatico();
                    Console.WriteLine("\nPressione qualquer tecla para voltar ao menu...");
                    Console.ReadKey();
                }

            } while (opcao != 11);

            // Salvar produtos ao sair do sistema
            SalvarProdutos();
        }

        static void ExibirMensagemErro(string mensagem)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nERRO: {mensagem}");
            Console.ResetColor();
        }

        static void ExibirMensagemSucesso(string mensagem)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nSUCESSO: {mensagem}");
            Console.ResetColor();
        }

        static void CadastrarProduto()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=== CADASTRO DE PRODUTOS ===");
            Console.ResetColor();

            Produto novoProduto;

            Console.Write("Digite o código do produto: ");
            while (!int.TryParse(Console.ReadLine(), out novoProduto.Codigo))
            {
                ExibirMensagemErro("Código inválido. Digite um número válido.");
            }

            Console.Write("Digite a descrição do produto: ");
            novoProduto.Descricao = Console.ReadLine();

            Console.Write("Digite o preço do produto: ");
            while (!float.TryParse(Console.ReadLine(), out novoProduto.Preco))
            {
                ExibirMensagemErro("Preço inválido. Digite um número válido.");
            }

            Console.Write("Digite a validade do produto (dd/MM/yyyy): ");
            while (!DateTime.TryParse(Console.ReadLine(), out novoProduto.Validade))
            {
                ExibirMensagemErro("Data inválida. Digite no formato dd/MM/yyyy.");
            }

            Console.Write("Digite a quantidade em estoque: ");
            while (!int.TryParse(Console.ReadLine(), out novoProduto.Quantidade))
            {
                ExibirMensagemErro("Quantidade inválida. Digite um número válido.");
            }

            listaProdutos.Add(novoProduto);
            listaProdutos.Sort((p1, p2) => p1.Descricao.CompareTo(p2.Descricao)); // Ordena por descrição
            ExibirMensagemSucesso("Produto cadastrado com sucesso!");

            SalvarProdutos(); // Salvar produtos após cadastro
        }

        static void ListarProdutos()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=== LISTA DE PRODUTOS ===");
            Console.ResetColor();

            if (listaProdutos.Count == 0)
            {
                ExibirMensagemErro("Nenhum produto cadastrado.");
                return;
            }

            foreach (var produto in listaProdutos)
            {
                Console.WriteLine($"Código: {produto.Codigo}");
                Console.WriteLine($"Descrição: {produto.Descricao}");
                Console.WriteLine($"Preço: R${produto.Preco:F2}");
                Console.WriteLine($"Validade: {produto.Validade:dd/MM/yyyy}");
                Console.WriteLine($"Quantidade: {produto.Quantidade}");
                if (produto.Validade < DateTime.Now)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Status: PRODUTO VENCIDO");
                    Console.ResetColor();
                }
                Console.WriteLine("----------------------------");
            }
        }

        static void BuscarProduto()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=== BUSCAR PRODUTO ===");
            Console.ResetColor();

            Console.Write("Digite o código do produto: ");
            if (!int.TryParse(Console.ReadLine(), out int codigoBusca))
            {
                ExibirMensagemErro("Código inválido.");
                return;
            }

            listaProdutos.Sort((p1, p2) => p1.Codigo.CompareTo(p2.Codigo)); // Ordena por código para busca binária
            int indice = listaProdutos.BinarySearch(new Produto { Codigo = codigoBusca }, Comparer<Produto>.Create((x, y) => x.Codigo.CompareTo(y.Codigo)));

            if (indice >= 0)
            {
                var produto = listaProdutos[indice];
                Console.WriteLine($"Produto encontrado:");
                Console.WriteLine($"Código: {produto.Codigo}");
                Console.WriteLine($"Descrição: {produto.Descricao}");
                Console.WriteLine($"Preço: R${produto.Preco:F2}");
                Console.WriteLine($"Validade: {produto.Validade:dd/MM/yyyy}");
                Console.WriteLine($"Quantidade: {produto.Quantidade}");
                if (produto.Validade < DateTime.Now)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Status: PRODUTO VENCIDO");
                    Console.ResetColor();
                }
            }
            else
            {
                ExibirMensagemErro("Produto não encontrado.");
            }
        }

        static void VerificarEstoqueBaixoRecursivo(int index)
        {
            // Caso base: se o índice for maior ou igual ao tamanho da lista, encerra a recursão
            if (index >= listaProdutos.Count)
                return;

            // Verifica se o produto tem estoque baixo
            var produto = listaProdutos[index];
            if (produto.Quantidade < 5)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Produto com estoque baixo: {produto.Descricao} | Estoque: {produto.Quantidade}");
                Console.ResetColor();
            }

            // Chamada recursiva para o próximo índice
            VerificarEstoqueBaixoRecursivo(index + 1);
        }

        static void VerificarProdutosVencidosRecursivo(int index)
        {
            // Caso base: se o índice for maior ou igual ao tamanho da lista, encerra a recursão
            if (index >= listaProdutos.Count)
                return;

            // Verifica se o produto está vencido
            var produto = listaProdutos[index];
            if (produto.Validade < DateTime.Now)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Produto vencido: {produto.Descricao} | Validade: {produto.Validade:dd/MM/yyyy}");
                Console.ResetColor();
            }

            // Chamada recursiva para o próximo índice
            VerificarProdutosVencidosRecursivo(index + 1);
        }

        static void VerificarEstoqueAutomatico()
        {
            foreach (var produto in listaProdutos)
            {
                if (produto.Quantidade < 5)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"\nALERTA AUTOMÁTICO: O produto \"{produto.Descricao}\" está com estoque baixo (Quantidade: {produto.Quantidade})");
                    Console.ResetColor();
                }
            }
        }

        static void RegistrarVenda()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=== REGISTRAR VENDA ===");
            Console.ResetColor();

            Console.Write("Digite o código do produto: ");
            if (!int.TryParse(Console.ReadLine(), out int codigo))
            {
                ExibirMensagemErro("Código inválido.");
                return;
            }

            var produtoIndex = listaProdutos.FindIndex(p => p.Codigo == codigo);
            if (produtoIndex == -1)
            {
                ExibirMensagemErro("Produto não encontrado!");
                return;
            }

            Produto produto = listaProdutos[produtoIndex];
            RegistrarVenda(ref produto);
            listaProdutos[produtoIndex] = produto; // Atualiza a lista com o produto modificado
            SalvarProdutos(); // Salvar produtos após a venda
        }

        static void RegistrarVenda(ref Produto produto)
        {
            if (produto.Validade < DateTime.Now)
            {
                ExibirMensagemErro("Não é possível vender um produto vencido.");
                return;
            }

            if ((produto.Validade - DateTime.Now).TotalDays <= 7)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("AVISO: O produto está próximo do vencimento (menos de 7 dias)!");
                Console.ResetColor();
            }

            Console.Write($"Digite a quantidade vendida de {produto.Descricao}: ");
            if (!int.TryParse(Console.ReadLine(), out int quantidadeVendida) || quantidadeVendida <= 0)
            {
                ExibirMensagemErro("Quantidade inválida.");
                return;
            }

            if (quantidadeVendida > produto.Quantidade)
            {
                ExibirMensagemErro("Estoque insuficiente!");
                return;
            }

            produto.Quantidade -= quantidadeVendida;

            using (StreamWriter writer = new StreamWriter(caminhoRegistroVendas, true))
            {
                writer.WriteLine($"Data: {DateTime.Now:dd/MM/yyyy}");
                writer.WriteLine($"Produto: {produto.Descricao} | Quantidade: {quantidadeVendida} | Total: R${produto.Preco * quantidadeVendida:F2}");
                writer.WriteLine("----------------------------");
            }

            ExibirMensagemSucesso("Venda registrada com sucesso!");
        }

        static void ReporEstoque()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=== REPOR ESTOQUE ===");
            Console.ResetColor();

            Console.Write("Digite o código do produto: ");
            if (!int.TryParse(Console.ReadLine(), out int codigo))
            {
                ExibirMensagemErro("Código inválido.");
                return;
            }

            var produtoIndex = listaProdutos.FindIndex(p => p.Codigo == codigo);
            if (produtoIndex == -1)
            {
                ExibirMensagemErro("Produto não encontrado!");
                return;
            }

            Produto produto = listaProdutos[produtoIndex];
            ReporEstoque(ref produto);
            listaProdutos[produtoIndex] = produto; // Atualiza a lista com o produto modificado
            SalvarProdutos(); // Salvar produtos após reposição
        }

        static void ReporEstoque(ref Produto produto)
        {
            Console.Write($"Digite a quantidade a ser adicionada ao estoque de {produto.Descricao}: ");
            if (!int.TryParse(Console.ReadLine(), out int quantidadeAdicionada) || quantidadeAdicionada <= 0)
            {
                ExibirMensagemErro("Quantidade inválida.");
                return;
            }

            if (produto.Validade < DateTime.Now)
            {
                Console.Write("Produto vencido! Digite a nova validade do produto (dd/MM/yyyy): ");
                DateTime novaValidade;
                while (!DateTime.TryParse(Console.ReadLine(), out novaValidade) || novaValidade < DateTime.Now)
                {
                    ExibirMensagemErro("Data inválida ou já expirada. Digite uma data futura.");
                }
                produto.Validade = novaValidade;
            }

            produto.Quantidade += quantidadeAdicionada;

            ExibirMensagemSucesso("Estoque atualizado com sucesso!");
        }

        static void GerarRelatorioProdutos()
        {
            string caminho = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "RelatorioProdutos.txt");

            using (StreamWriter writer = new StreamWriter(caminho))
            {
                writer.WriteLine("=== RELATÓRIO DE PRODUTOS ===");
                foreach (var produto in listaProdutos)
                {
                    writer.WriteLine($"Código: {produto.Codigo}");
                    writer.WriteLine($"Descrição: {produto.Descricao}");
                    writer.WriteLine($"Preço: R${produto.Preco:F2}");
                    writer.WriteLine($"Validade: {produto.Validade:dd/MM/yyyy}");
                    writer.WriteLine($"Quantidade: {produto.Quantidade}");
                    if (produto.Validade < DateTime.Now)
                    {
                        writer.WriteLine("Status: PRODUTO VENCIDO");
                    }
                    writer.WriteLine("----------------------------");
                }
            }

            ExibirMensagemSucesso($"Relatório gerado na área de trabalho: {caminho}");
        }

        static void ConsultarRegistroDeVendas()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=== CONSULTAR REGISTRO DE VENDAS ===");
            Console.ResetColor();

            if (!File.Exists(caminhoRegistroVendas))
            {
                ExibirMensagemErro("Nenhum registro de vendas encontrado.");
                return;
            }

            string[] registros = File.ReadAllLines(caminhoRegistroVendas);
            foreach (string linha in registros)
            {
                Console.WriteLine(linha);
            }
        }

        static void GerarRelatorioFluxoDeCaixa()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=== GERAR RELATÓRIO DE FLUXO DE CAIXA ===");
            Console.ResetColor();

            if (!File.Exists(caminhoRegistroVendas))
            {
                ExibirMensagemErro("Nenhum registro de vendas encontrado.");
                return;
            }

            Dictionary<string, float> fluxoDeCaixa = new Dictionary<string, float>();

            string[] registros = File.ReadAllLines(caminhoRegistroVendas);
            foreach (string linha in registros)
            {
                if (linha.StartsWith("Data:"))
                {
                    string data = linha.Substring(6);
                    if (!fluxoDeCaixa.ContainsKey(data))
                    {
                        fluxoDeCaixa[data] = 0;
                    }
                }
                else if (linha.StartsWith("Produto:"))
                {
                    string[] partes = linha.Split('|');
                    string totalString = partes[2].Split(':')[1].Trim().Replace("R$", "");
                    float total = float.Parse(totalString);
                    string data = registros[Array.IndexOf(registros, linha) - 1].Substring(6);
                    fluxoDeCaixa[data] += total;
                }
            }

            string caminhoFluxoCaixa = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "FluxoDeCaixa.txt");
            using (StreamWriter writer = new StreamWriter(caminhoFluxoCaixa))
            {
                writer.WriteLine("=== RELATÓRIO DE FLUXO DE CAIXA ===");
                foreach (var item in fluxoDeCaixa)
                {
                    writer.WriteLine($"Data: {item.Key} | Total de Vendas: R${item.Value:F2}");
                }
            }

            ExibirMensagemSucesso($"Relatório de Fluxo de Caixa gerado na área de trabalho: {caminhoFluxoCaixa}");
        }

        static void SalvarProdutos()
        {
            try
            {
                using (FileStream fs = new FileStream(caminhoArquivoBinario, FileMode.Create))
                using (BinaryWriter writer = new BinaryWriter(fs))
                {
                    writer.Write(listaProdutos.Count);
                    foreach (var produto in listaProdutos)
                    {
                        writer.Write(produto.Codigo);
                        writer.Write(produto.Descricao);
                        writer.Write(produto.Preco);
                        writer.Write(produto.Validade.ToBinary());
                        writer.Write(produto.Quantidade);
                    }
                }
            }
            catch (Exception ex)
            {
                ExibirMensagemErro($"Erro ao salvar produtos: {ex.Message}");
            }
        }

        static void CarregarProdutos()
        {
            try
            {
                if (File.Exists(caminhoArquivoBinario))
                {
                    using (FileStream fs = new FileStream(caminhoArquivoBinario, FileMode.Open))
                    using (BinaryReader reader = new BinaryReader(fs))
                    {
                        int quantidadeProdutos = reader.ReadInt32();
                        listaProdutos.Clear();
                        for (int i = 0; i < quantidadeProdutos; i++)
                        {
                            Produto produto = new Produto
                            {
                                Codigo = reader.ReadInt32(),
                                Descricao = reader.ReadString(),
                                Preco = reader.ReadSingle(),
                                Validade = DateTime.FromBinary(reader.ReadInt64()),
                                Quantidade = reader.ReadInt32()
                            };
                            listaProdutos.Add(produto);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExibirMensagemErro($"Erro ao carregar produtos: {ex.Message}");
            }
        }
    }
}