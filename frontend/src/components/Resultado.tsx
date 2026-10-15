import type { BuscaResponse } from '../types.ts'

interface ResultadoProps {
  dados: BuscaResponse
}

function Resultado({ dados }: ResultadoProps) {
  const totalElegiveis = dados.professores.length

  return (
    <>
      <section>
        <div className="secao-cabecalho">
          <h2>Professores disponíveis</h2>
          <span className="contador">
            {`${totalElegiveis} ${totalElegiveis === 1 ? 'professor' : 'professores'}`}
          </span>
        </div>

        {totalElegiveis === 0 ? (
          <div className="painel painel-vazio">
            <p className="painel-titulo">Nenhum professor encontrado</p>
            <p>Nenhum professor atende a todos os critérios informados. Ajuste a busca e tente novamente.</p>
          </div>
        ) : (
          <ul className="professores">
            {dados.professores.map((professor) => (
              <li key={professor.id} className="professor">
                <div className="professor-cabecalho">
                  <span className="avatar" aria-hidden="true">
                    {professor.nome.charAt(0)}
                  </span>
                  <div>
                    <h3>{professor.nome}</h3>
                    <p className="aulas">
                      {`${professor.aulasNaSemana} ${professor.aulasNaSemana === 1 ? 'aula' : 'aulas'} na semana`}
                    </p>
                  </div>
                </div>
                <div className="horarios-bloco">
                  <p className="rotulo">Horários possíveis</p>
                  <ul className="horarios">
                    {professor.horarios.map((horario) => (
                      <li key={horario}>{horario}</li>
                    ))}
                  </ul>
                </div>
              </li>
            ))}
          </ul>
        )}
      </section>

      {dados.naoElegiveis.length > 0 && (
        <section className="secao-secundaria">
          <div className="secao-cabecalho">
            <h2>Professores que ficaram de fora</h2>
            <span className="contador contador-discreto">{dados.naoElegiveis.length}</span>
          </div>
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
