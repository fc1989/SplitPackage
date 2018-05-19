<template>
    <div>
        <Table :columns="columns" border :data="logisticRelated.items"></Table>
    </div>
</template>
<script>
    export default {
        props: {
            logisticRelated: Object
        },
        data(){
            return {
                columns: [{
                    title: this.$t('Menu.Pages.Logistics'),
                    key: 'logisticName'
                }]
            }
        }
    };
</script>