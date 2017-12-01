declare module Brreg {
    export interface Adresse {
        adresse: string;
        postnummer: string;
        poststed: string;
        kommunenummer: string;
        kommune: string;
        landkode: string;
        land: string;
    }

    export interface Næringskode {
        kode: string;
        beskrivelse: string;
    }

    export class BrregCompany {
        organisasjonsnummer: number;
        navn: string;
        organisasjonsform: string;
        registreringsdatoEnhetsregisteret: string;
        registrertIFrivillighetsregisteret: string;
        registrertIMvaregisteret: string;
        registrertIForetaksregisteret: string;
        registrertIStiftelsesregisteret: string;
        konkurs: string;
        underAvvikling: string;
        underTvangsavviklingEllerTvangsopplosning: string;
        postadresse: Adresse;
        forretningsadresse: Adresse;
        naeringskode1: Næringskode;
        naeringskode2: Næringskode;
        naeringskode3: Næringskode;
    }
}