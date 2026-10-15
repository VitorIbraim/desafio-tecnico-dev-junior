import type { BuscaResponse } from '../types.ts'

interface ResultadoProps {
  dados: BuscaResponse
}

function Resultado({ dados }: ResultadoProps) {
  return (
    <>
      <section className="secao">
        <h2>Professores disponíveis</h2>
        {dados.professores.length === 0 ? (
          <p className="aviso">Nenhum professor encontrado para os critérios informados.</p>
        ) : (
          <ul className="professores">
            {dados.professores.map((professor) => (
              <li key={professor.id} className="professor">
                <div className="professor-cabecalho">
                  <h3>{professor.nome}</h3>
                  <span className="aulas">
                    {`${professor.aulasNaSemana} ${professor.aulasNaSemana === 1 ? 'aula' : 'aulas'} na semana`}
                  </span>
                </div>
                <p className="rotulo">Horários possíveis</p>
                <ul className="horarios">
                  {professor.horarios.map((horario) => (
                    <li key={horario}>{horario}</li>
                  ))}
                </ul>
              </li>
            ))}
          </ul>
        )}
      </section>

      {dados.naoElegiveis.length > 0 && (
        <section className="secao">
          <h2>Professores que ficaram de fora</h2>
          <ul className="nao-elegiveis">
            {dados.naoElegiveis.map((professor) => (
              <li key={professor.id}>
                <span className="nome">{professor.nome}</span>
                <span className="motivo">{professor.motivo}</span>
              </li>
            ))}
          </ul>
        </section>
      )}
    </>
  )
}

export default Resultado
