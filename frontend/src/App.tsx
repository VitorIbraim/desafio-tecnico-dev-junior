import { useState } from 'react'
import { buscarProfessores } from './api.ts'
import BuscaForm from './components/BuscaForm.tsx'
import Resultado from './components/Resultado.tsx'
import type { BuscaRequest, BuscaResponse } from './types.ts'

type Estado =
  | { tipo: 'inicial' }
  | { tipo: 'carregando' }
  | { tipo: 'sucesso'; dados: BuscaResponse }
  | { tipo: 'validacao'; erros: string[] }
  | { tipo: 'falha' }

function App() {
  const [estado, setEstado] = useState<Estado>({ tipo: 'inicial' })

  async function buscar(request: BuscaRequest) {
    setEstado({ tipo: 'carregando' })

    try {
      setEstado(await buscarProfessores(request))
    } catch {
      setEstado({ tipo: 'falha' })
    }
  }

  return (
    <main className="pagina">
      <header className="cabecalho">
        <h1>Encontre um Professor</h1>
        <p>Informe o idioma, os dias, a faixa de horário e a duração da aula.</p>
      </header>

      <BuscaForm carregando={estado.tipo === 'carregando'} onBuscar={buscar} />

      <div className="resultado" aria-live="polite">
        {estado.tipo === 'inicial' && (
          <div className="painel painel-neutro">
            <p className="painel-titulo">Nenhuma busca realizada</p>
            <p>Preencha os critérios acima e clique em buscar professores.</p>
          </div>
        )}

        {estado.tipo === 'carregando' && (
          <div className="painel painel-carregando">
            <p className="painel-titulo">Buscando professores...</p>
          </div>
        )}

        {estado.tipo === 'validacao' && (
          <div className="painel alerta">
            <p className="painel-titulo">Verifique os dados da busca</p>
            <ul>
              {estado.erros.map((erro, indice) => (
                <li key={indice}>{erro}</li>
              ))}
            </ul>
          </div>
        )}

        {estado.tipo === 'falha' && (
          <div className="painel alerta">
            <p className="painel-titulo">Não foi possível se comunicar com a API</p>
            <p>Verifique se ela está em execução e tente novamente.</p>
          </div>
        )}

        {estado.tipo === 'sucesso' && <Resultado dados={estado.dados} />}
      </div>
    </main>
  )
}

export default App
