export interface BuscaRequest {
  idioma: string
  dias: string[]
  inicioFaixa: string
  fimFaixa: string
  duracaoMinutos: number | null
}

export interface ProfessorElegivel {
  id: number
  nome: string
  aulasNaSemana: number
  horarios: string[]
}

export interface ProfessorNaoElegivel {
  id: number
  nome: string
  motivo: string
}

export interface BuscaResponse {
  professores: ProfessorElegivel[]
  naoElegiveis: ProfessorNaoElegivel[]
}
