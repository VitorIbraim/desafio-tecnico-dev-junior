import { useState } from 'react'
import type { FormEvent } from 'react'
import type { BuscaRequest } from '../types.ts'

const IDIOMAS = ['Inglês', 'Espanhol', 'Francês']

const DIAS = [
  { valor: 'segunda', rotulo: 'Segunda' },
  { valor: 'terca', rotulo: 'Terça' },
  { valor: 'quarta', rotulo: 'Quarta' },
  { valor: 'quinta', rotulo: 'Quinta' },
  { valor: 'sexta', rotulo: 'Sexta' },
  { valor: 'sabado', rotulo: 'Sábado' },
  { valor: 'domingo', rotulo: 'Domingo' },
]

const DURACOES = [30, 60, 90]

interface BuscaFormProps {
  carregando: boolean
  onBuscar: (request: BuscaRequest) => void
}

function BuscaForm({ carregando, onBuscar }: BuscaFormProps) {
  const [idioma, setIdioma] = useState('')
  const [dias, setDias] = useState<string[]>([])
  const [inicioFaixa, setInicioFaixa] = useState('')
  const [fimFaixa, setFimFaixa] = useState('')
  const [duracao, setDuracao] = useState('')

  function alternarDia(dia: string) {
    setDias((atuais) =>
      atuais.includes(dia) ? atuais.filter((atual) => atual !== dia) : [...atuais, dia],
    )
  }

  function enviar(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    onBuscar({
      idioma,
      dias: DIAS.map((dia) => dia.valor).filter((dia) => dias.includes(dia)),
      inicioFaixa,
      fimFaixa,
      duracaoMinutos: duracao === '' ? null : Number(duracao),
    })
  }

  return (
    <form className="formulario" onSubmit={enviar}>
      <div className="campo">
        <label htmlFor="idioma">Idioma</label>
        <select id="idioma" value={idioma} onChange={(event) => setIdioma(event.target.value)}>
          <option value="">Selecione</option>
          {IDIOMAS.map((opcao) => (
            <option key={opcao} value={opcao}>
              {opcao}
            </option>
          ))}
        </select>
      </div>

      <div className="campo">
        <label htmlFor="duracao">Duração da aula</label>
        <select id="duracao" value={duracao} onChange={(event) => setDuracao(event.target.value)}>
          <option value="">Selecione</option>
          {DURACOES.map((minutos) => (
            <option key={minutos} value={minutos}>
              {minutos} minutos
            </option>
          ))}
        </select>
      </div>

      <fieldset className="campo campo-largo">
        <legend>Dias da semana</legend>
        <div className="dias">
          {DIAS.map((dia) => (
            <label key={dia.valor} className="dia">
              <input
                type="checkbox"
                checked={dias.includes(dia.valor)}
                onChange={() => alternarDia(dia.valor)}
              />
              {dia.rotulo}
            </label>
          ))}
        </div>
      </fieldset>

      <div className="campo">
        <label htmlFor="inicioFaixa">Início da faixa</label>
        <input
          id="inicioFaixa"
          type="time"
          value={inicioFaixa}
          onChange={(event) => setInicioFaixa(event.target.value)}
        />
      </div>

      <div className="campo">
        <label htmlFor="fimFaixa">Fim da faixa</label>
        <input
          id="fimFaixa"
          type="time"
          value={fimFaixa}
          onChange={(event) => setFimFaixa(event.target.value)}
        />
      </div>

      <div className="campo-largo">
        <button type="submit" disabled={carregando}>
          {carregando ? 'Buscando...' : 'Buscar professores'}
        </button>
      </div>
    </form>
  )
}

export default BuscaForm
