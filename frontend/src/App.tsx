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
          <p className="aviso">Preencha os critérios e clique em buscar.</p>
        )}

        {estado.tipo === 'carregando' && <p className="aviso">Buscando professores...</p>}

        {estado.tipo === 'validacao' && (
          <div className="alerta">
            <p>Verifique os dados da busca:</p>
            <ul>
              {estado.erros.map((erro, indice) => (
                <li key={indice}>{erro}</li>
              ))}
            </ul>
          </div>
        )}

        {estado.tipo === 'falha' && (
          <div className="alerta">
            <p>Não foi possível se comunicar com a API. Verifique se ela está em execução e tente novamente.</p>
          </div>
        )}

        {estado.tipo === 'sucesso' && <Resultado dados={estado.dados} />}
      </div>
    </main>
  )
}

export default App
